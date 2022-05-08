#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using System;
using UnityEditor;
using qASIC.Tools;
using qASIC.EditorTools;
using System.Linq;

using WindowUtility = qASIC.InputManagement.Map.Internal.InputMapWindowEditorUtility;

namespace qASIC.InputManagement.Map.Internal
{
    public class InputMapWindowContentTree : TreeView
    {
		public InputMapTreeActionHeaderItem ActionsRoot { get; private set; }
		public InputMapTreeAxisHeaderItem AxesRoot { get; private set; }

		private InputGroup group;
		public InputGroup Group
		{
			get => group;
			set => group = value;
		}

		//The context menu has to be shown on next repaint in order
		//for the item to get selected
		bool showContextOnNextRepaint;

		Color HeaderBarColor => Color.clear;

		event Action OnNextRepaint;

        #region Creating
        public InputMapWindowContentTree(TreeViewState state, InputGroup group)
			: base(state)
		{
			this.group = group;
			Reload();
		}

		protected override TreeViewItem BuildRoot()
		{
			return new TreeViewItem { id = 0, depth = -1 };
		}

		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			var rows = GetRows() ?? new List<TreeViewItem>();

			rows.Clear();

            if (Group != null)
            {
				CreateActionItems(root, rows);
				CreateAxisItems(root, rows);
			}

            SetupDepthsFromParentsAndChildren(root);
			return rows;
		}

		void CreateActionItems(TreeViewItem root, IList<TreeViewItem> rows)
        {
			ActionsRoot = new InputMapTreeActionHeaderItem(1, -1, "Actions", HeaderBarColor);
			root.AddChild(ActionsRoot);
			rows.Add(ActionsRoot);

			if (IsExpanded(ActionsRoot.id))
			{
				for (int i = 0; i < Group.actions.Count; i++)
				{
                    TreeViewItem item = new InputMapContentActionTreeItem(Group.actions[i], i + 2);
                    ActionsRoot.AddChild(item);
					rows.Add(item);
				}
				return;
			}

			if(Group.actions.Count != 0)
				ActionsRoot.children = CreateChildListForCollapsedParent();
		}

		void CreateAxisItems(TreeViewItem root, IList<TreeViewItem> rows)
        {
			AxesRoot = new InputMapTreeAxisHeaderItem(-1, -1, "Axes", HeaderBarColor);
			root.AddChild(AxesRoot);
			rows.Add(AxesRoot);

			if (IsExpanded(AxesRoot.id))
            {
                for (int i = 0; i < Group.axes.Count; i++)
                {
					TreeViewItem item = new InputMapContentAxisTreeItem(Group.axes[i], i + 2 + Group.actions.Count);
					AxesRoot.AddChild(item);
					rows.Add(item);
                }
				return;
            }

			if (Group.axes.Count != 0)
				AxesRoot.children = CreateChildListForCollapsedParent();
        }
		#endregion

		#region Selecting
		public event Action<object> OnItemSelect;

		//TODO: Add multi select support. This will require rewriting how the
		//context menu gets displayed and used.
		protected override bool CanMultiSelect(TreeViewItem item) => false;

        protected override void SelectionChanged(IList<int> selectedIds)
        {
			if(selectedIds.Count != 1)
            {
				OnItemSelect?.Invoke(null);
				return;
            }

			SelectItemInInspector(selectedIds[0]);
		}

        public TreeViewItem GetSelectedContentItem()
		{
			IList<int> selection = GetSelection();

			if (selection.Count != 1)
				return null;

			return FindItem(selection[0], rootItem);
		}

        protected override void SingleClickedItem(int id)
        {
			SelectItemInInspector(id);
		}

		protected override void DoubleClickedItem(int id)
		{
			TreeViewItem item = FindItem(id, rootItem);
			if (CanRename(item))
				BeginRename(item);
		}

		protected override void ContextClickedItem(int id)
		{
			showContextOnNextRepaint = true;
			Repaint();
		}

		void SelectItemInInspector(int id) =>
			SelectItemInInspector(FindItem(id, rootItem));

		void SelectItemInInspector(TreeViewItem item)
        {
			switch (item)
			{
				case InputMapContentActionTreeItem action:
					OnItemSelect?.Invoke(new InputMapWindowInspector.InspectorInputAction(Group, action.Action));
					break;
				case InputMapContentAxisTreeItem axis:
					OnItemSelect?.Invoke(new InputMapWindowInspector.InspectorInputAxis(Group, axis.Axis));
					break;
				default:
					OnItemSelect?.Invoke(null);
					break;
			}
		}
		#endregion

		#region Expanding
		public event Action OnExpand;

        protected override void ExpandedStateChanged()
        {
			base.ExpandedStateChanged();
			OnExpand?.Invoke();
		}
        #endregion

        #region Context menu
        void ShowContextMenu()
		{
			TreeViewItem item = GetSelectedContentItem();

			GenericMenu menu = new GenericMenu();

			switch (item)
			{
				case InputMapContentActionTreeItem action:
					AddActionGenericMenuItems(menu, action);
					menu.AddSeparator("");
					break;
				case InputMapTreeActionHeaderItem _:
					AddActionGenericMenuItems(menu, null);
					menu.AddSeparator("");
					break;
				case InputMapContentAxisTreeItem axis:
					AddAxisGenericMenuItems(menu, axis);
					menu.AddSeparator("");
					break;
				case InputMapTreeAxisHeaderItem _:
					AddAxisGenericMenuItems(menu, null);
					menu.AddSeparator("");
					break;
			}

			AddEditGenericMenuItems(menu, item);
			menu.ShowAsContext();
		}

		void AddActionGenericMenuItems(GenericMenu menu, InputMapContentActionTreeItem action)
        {
			menu.AddItem("Add", false, () => AddActionItem(action));
		}

		void AddAxisGenericMenuItems(GenericMenu menu, InputMapContentAxisTreeItem axis)
		{
			menu.AddItem("Add", false, () => AddAxisItem(axis));
		}

		void AddEditGenericMenuItems(GenericMenu menu, TreeViewItem item)
		{
			InputMapContentEditableItemBase editableItem = item as InputMapContentEditableItemBase;
			bool editable = !(editableItem is null);

			menu.AddToggableItem("Rename", false, () => BeginRename(item), editable);
			menu.AddToggableItem("Delete", false, () => Delete(editableItem), editable);

			menu.AddSeparator("");
			menu.AddItem("Expand all", false, ExpandAll);
			menu.AddItem("Collapse all", false, CollapseAll);
		}
		#endregion

		#region Adding
		void AddActionItem(InputMapContentActionTreeItem action)
        {
			AddItem(Group.actions, action?.Action, new InputAction(WindowUtility.GenerateUniqueName("New action", (string s) =>
			{
				return NonRepeatableChecker.ContainsKey(Group.actions, s);
			})),
			//Expand action
			() =>
            {
				SetExpanded(ActionsRoot.id, true);
			});
		}

		void AddAxisItem(InputMapContentAxisTreeItem axis)
        {
			AddItem(Group.axes, axis?.Axis, new InputAxis(WindowUtility.GenerateUniqueName("New axis", (string s) =>
			{
				return NonRepeatableChecker.ContainsKey(Group.axes, s);
			})),
			//Expand action
			() =>
			{
				SetExpanded(AxesRoot.id, true);
			});
		}

		void AddItem<t>(List<t> list, t selectedObject, t item, Action ExpandAction = null)
		{
			int index = list.IndexOf(selectedObject);
			if (index == -1)
				index = list.Count - 1;
			list.Insert(index + 1, item);
			InputMapWindow.SetMapDirty();
			Reload();

			ExpandAction?.Invoke();
			Repaint();
			OnNextRepaint += () =>
			{
				InputMapContentEditableItemBase treeItem = GetItemByContent(item);
				if (treeItem != null)
					BeginRename(treeItem);
			};
		}
        #endregion

        #region Copy&Paste&Move&Delete
		void Delete(InputMapContentEditableItemBase item)
        {
			item.Delete(Group);
			InputMapWindow.SetMapDirty();
			InputMapWindow.GetEditorWindow().SelectInInspector(null);
			SetSelection(new List<int>());
			Reload();
        }

		//TODO: maybe one day
  //      protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
  //      {
		//	DragAndDrop.PrepareStartDrag();
		//	DragAndDrop.SetGenericData("itemID", args.draggedItemIDs[0]);
		//	DragAndDrop.SetGenericData("tree", this);
		//	DragAndDrop.StartDrag(FindItem(args.draggedItemIDs[0], rootItem).displayName);
		//}

		//protected override bool CanStartDrag(CanStartDragArgs args)
		//{
		//	if (!(args.draggedItem is InputMapContentItemBase item))
		//		return false;

		//	return item.CanDrag;
		//}

		//protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
  //      {
		//	if (!(DragAndDrop.GetGenericData("tree") is InputMapWindowContentTree sourceTree))
		//		return DragAndDropVisualMode.Rejected;

		//	int itemID = (int)DragAndDrop.GetGenericData("itemID");
		//	var item = FindItem(itemID, rootItem);

		//	if (!(args.parentItem is InputMapContentHeaderItemBase))
		//		return DragAndDropVisualMode.Rejected;

		//	if (args.performDrop)
  //          {
		//		switch (item)
  //              {
		//			case InputMapContentActionTreeItem actionItem:

		//				break;
		//			case InputMapContentAxisTreeItem axisItem:
		//				break;
  //              }
  //          }				

		//	return DragAndDropVisualMode.Move;
  //      }
        #endregion

        #region Finding
        InputMapContentEditableItemBase GetItemByContent<t>(t item)
        {
			IList<int> items = GetDescendantsThatHaveChildren(rootItem.id);

            for (int i = 0; i < items.Count; i++)
				if (FindItem(items[i], rootItem) is InputMapContentEditableItemBase editable && editable.CompareContent(item))
					return editable;

			return null;
        }
        #endregion

        #region Renaming
        protected override bool CanRename(TreeViewItem item)
        {
            return item is InputMapContentEditableItemBase;
        }

        protected override void RenameEnded(RenameEndedArgs args)
		{
			if (!args.acceptedRename) return;
			if (string.IsNullOrWhiteSpace(args.newName)) return;

			switch (FindItem(args.itemID, rootItem))
			{
				case InputMapContentActionTreeItem item:
					if (NonRepeatableChecker.ContainsKey(Group.actions, args.newName)) return;
					item.Rename(args.newName);
					break;
				case InputMapContentAxisTreeItem item:
					if (NonRepeatableChecker.ContainsKey(Group.axes, args.newName)) return;
					item.Rename(args.newName);
					break;
				default:
					return;
			}

			InputMapWindow.SetMapDirty();
			Reload();
		}
		#endregion

		#region GUI
		protected override void RowGUI(RowGUIArgs args)
		{
			bool repaint = Event.current.type == EventType.Repaint;

			if (OnNextRepaint != null && repaint)
            {
				OnNextRepaint?.Invoke();
				OnNextRepaint = null;
            }

			if (showContextOnNextRepaint && repaint)
            {
				ShowContextMenu();
				showContextOnNextRepaint = false;
            }

			//Button (either plus or minus)
			Rect buttonRect = new Rect(args.rowRect).ResizeToRight(GetCustomRowHeight(args.row, args.item));

			GUIContent buttonContent = new GUIContent();

			switch (args.item)
            {
				case InputMapContentEditableItemBase _:
					buttonContent.image = qGUIEditorUtility.MinusIcon;
					break;
				case InputMapContentHeaderItemBase _:
					buttonContent.image = qGUIEditorUtility.PlusIcon;
					break;
            }

			if (GUI.Button(buttonRect, buttonContent, Styles.Label))
			{
				switch (args.item)
				{
					case InputMapTreeActionHeaderItem _:
						AddActionItem(null);
						break;
					case InputMapTreeAxisHeaderItem _:
						AddAxisItem(null);
						break;
					case InputMapContentEditableItemBase item:
						Delete(item);
						break;
				}
			}

			if (repaint)
				OnGUIRepaint(args);
		}

		void OnGUIRepaint(RowGUIArgs args)
        {
			InputMapContentItemBase item = args.item as InputMapContentItemBase;

			//Label
			Rect rowRect = new Rect(args.rowRect).MoveRight(GetContentIndent(args.item));

			GUIContent labelContent = new GUIContent(args.label);
			if (InputMapWindow.Prefs_ShowItemIcons)
            {
				labelContent.image = item.GetIcon(group);
				labelContent.tooltip = item.GetTooltip(group);
            }

			Styles.Label.Draw(rowRect, labelContent, false, false, args.selected, args.focused);

			//Separator
			Rect separatorRect = new Rect(rowRect).ResizeToBottom(1f);
			Styles.Separator.Draw(separatorRect, GUIContent.none, true, true, args.selected, args.focused);

            //Color bar
            if (!(item is null) && item.BarColor != Color.clear)
            {
                Rect barRect = new Rect(rowRect).ResizeToBottom(2f);
                Styles.ColorBar(item.BarColor).Draw(barRect, GUIContent.none, false, false, false, false);
            }

            CalcFoldoutOffset(rowRect.height);
		}

		//Centers foldout vertically
		void CalcFoldoutOffset(float height) =>
			customFoldoutYOffset = (height - 16f) / 2;

		protected override float GetCustomRowHeight(int row, TreeViewItem item)
		{
			switch (item)
			{
				case InputMapContentHeaderItemBase _:
					return 22f;
				default:
					return 18f;
			}
		}

		protected override IList<int> GetDescendantsThatHaveChildren(int id)
		{
			Stack<TreeViewItem> stack = new Stack<TreeViewItem>();

			var start = FindItem(id, rootItem);
			stack.Push(start);

			var parents = new List<int>();
			while (stack.Count > 0)
			{
				TreeViewItem item = stack.Pop();
				parents.Add(item.id);
				if (item.hasChildren)
					for (int i = 0; i < item.children.Count; i++)
						if (item.children[i] != null)
							stack.Push(item.children[i]);
			}

			return parents;
		}
        #endregion

		static class Styles
        {
			public static GUIStyle Label => new GUIStyle("Label") { alignment = TextAnchor.MiddleLeft };
			public static GUIStyle Separator => new GUIStyle("Label").WithBackground(qGUIEditorUtility.BorderTexture);
			public static GUIStyle ColorBar(Color color) => new GUIStyle("Label").WithBackgroundColor(color);
        }
    }
}
#endif