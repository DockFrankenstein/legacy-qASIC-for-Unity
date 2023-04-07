#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using System;
using UnityEditor;
using qASIC.EditorTools;
using System.Linq;

using WindowUtility = qASIC.Input.Map.Internal.InputMapWindowUtility;
using UnityEngine.UIElements;
using qASIC;

namespace qASIC.Input.Map.Internal
{
    public class InputMapWindowContentTree : TreeView
    {
		private const int BINDINGS_ROOT_ID = -1;
		private const int OTHERS_ROOT_ID = -2;

		public InputMapWindow window;

		public InputMapContentItemBase BindingsRoot { get; private set; }
		public InputMapContentItemBase OthersRoot { get; private set; }

		private InputGroup group;
		public InputGroup Group { get => group; set => group = value; }

		//The context menu has to be shown on next repaint in order
		//for the item to get selected
		bool _showContextOnNextRepaint;

		bool _isSearching;

		event Action _OnNextRepaint;

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
			_isSearching = !string.IsNullOrEmpty(searchString);
            var rows = GetRows() ?? new List<TreeViewItem>();

			rows.Clear();

            if (Group != null)
			{
				switch (_isSearching)
				{
					case true:
                        List<InputMapItem> items = qGUIEditorUtility.SortSearchList(Group.items, x => x.ItemName, searchString)
                            .ToList();

						foreach (var item in items)
						{
                            TreeViewItem treeItem = new InputMapContentMapItem(item);
                            root.AddChild(treeItem);
                            rows.Add(treeItem);
                        }

						BindingsRoot = null;
						OthersRoot = null;
						break;
					case false:
						CreateBindingItems(root, rows);
						CreateOtherItems(root, rows);
						break;
				}
			}

            SetupDepthsFromParentsAndChildren(root);
			return rows;
		}

		void CreateBindingItems(TreeViewItem root, IList<TreeViewItem> rows)
        {
			BindingsRoot = new InputMapContentBindingHeader(BINDINGS_ROOT_ID, -1);
			BindingsRoot.displayName = "Bindings";

			root.AddChild(BindingsRoot);
			rows.Add(BindingsRoot);

			List<InputBinding> bindings = Group.items
				.Where(x => x is InputBinding)
				.Select(x => x as InputBinding)
				.ToList();

			bindings = qGUIEditorUtility.SortSearchList(bindings, x => x.ItemName, searchString)
                .ToList();

			if (IsExpanded(BindingsRoot.id))
			{
				for (int i = 0; i < bindings.Count; i++)
				{
                    TreeViewItem item = new InputMapContentMapItem(bindings[i]);
                    BindingsRoot.AddChild(item);
					rows.Add(item);
				}
				return;
			}

			if (bindings.Count != 0)
				BindingsRoot.children = CreateChildListForCollapsedParent();
		}

		void CreateOtherItems(TreeViewItem root, IList<TreeViewItem> rows)
        {
			OthersRoot = new InputMapContentOtherHeader(OTHERS_ROOT_ID, -1);
			OthersRoot.displayName = "Items";

            root.AddChild(OthersRoot);
			rows.Add(OthersRoot);

            List<InputMapItem> items = Group.items
				.Where(x => !(x is InputBinding))
				.ToList();

			items = qGUIEditorUtility.SortSearchList(items, x => x.ItemName, searchString)
                .ToList();


			if (IsExpanded(OthersRoot.id))
			{
                for (int i = 0; i < items.Count; i++)
                {
					TreeViewItem item = new InputMapContentMapItem(items[i]);
					OthersRoot.AddChild(item);
					rows.Add(item);
                }

				return;
            }

			if (items.Count != 0)
				OthersRoot.children = CreateChildListForCollapsedParent();
        }
		#endregion

		#region Selecting
		//TODO: Add multi select support. This will require rewriting how the
		//context menu gets displayed and used.
		protected override bool CanMultiSelect(TreeViewItem item) => false;

        protected override void SelectionChanged(IList<int> selectedIds)
        {
			if (selectedIds.Count != 1)
            {
				window.SelectInInspector(null);
				return;
            }

			SelectItemInInspector(selectedIds[0]);
		}

        public InputMapContentItemBase GetSelectedContentItem()
		{
			IList<int> selection = GetSelection();

			if (selection.Count != 1)
				return null;

			return FindItem(selection[0], rootItem) as InputMapContentItemBase;
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
			_showContextOnNextRepaint = true;
			Repaint();
		}

		void SelectItemInInspector(int id) =>
			SelectItemInInspector(FindItem(id, rootItem));

		void SelectItemInInspector(TreeViewItem item)
        {
			SetSelection(new int[] { item.id });
			(item as InputMapContentItemBase)?.SelectInInspector(window);
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
			InputMapContentItemBase item = GetSelectedContentItem();

			GenericMenu menu = new GenericMenu();

			AddEditGenericMenuItems(menu, item);
			menu.ShowAsContext();
		}

		//void AddActionGenericMenuItems(GenericMenu menu, InputMapContentActionTreeItem action)
		//      {
		//	menu.AddItem("Add", false, () => AddActionItem(action));
		//}

		//void AddAxisGenericMenuItems(GenericMenu menu, InputMapContentOtherTreeItem axis)
		//{
		//	menu.AddItem("Add", false, () => AddAxisItem(axis));
		//}

		void AddEditGenericMenuItems(GenericMenu menu, InputMapContentItemBase item)
		{
			InputMapContentMapItem editableItem = item as InputMapContentMapItem;
			bool editable = !(editableItem is null);

			menu.AddToggableItem("Rename", false, () => BeginRename(item), editable);
			menu.AddToggableItem("Delete", false, () => Delete(editableItem), editable);

			menu.AddSeparator("");
			menu.AddItem("Expand all", false, ExpandAll);
			menu.AddItem("Collapse all", false, CollapseAll);

			item.CreateGenericMenu(menu);
		}
		#endregion

		#region Adding
		public void AddItem<T>() where T : InputMapItem =>
			AddItem(typeof(T));

        public void AddItem(Type type)
        {
            InputMapItem item = (InputMapItem)Activator.CreateInstance(type, new object[] { });
            AddItem(item);
        }

        public void AddItem(InputMapItem item)
		{
            item.ItemName = WindowUtility.GenerateUniqueName("New item", s => NonRepeatableChecker.ContainsKey(Group.items, s));

            switch (item)
            {
                case InputBinding binding:
                    binding.keys = new List<string>(InputMapWindow.Prefs_DefaultBindingKeys);
                    break;
            }

            var selectedItem = GetSelectedContentItem();

            int index = Group.items.Count - 1;

            if (selectedItem is InputMapContentMapItem mapItem)
			{
				int indexOf = Group.items.IndexOf(mapItem.Item);
				if (indexOf != -1)
					index = indexOf;
			}

			Group.InsertItem(index + 1, item);
            window.SetMapDirty();
			Reload();


			switch (item)
			{
				case InputBinding binding:
                    SetExpanded(BindingsRoot.id, true);
                    break;
				default:
                    SetExpanded(OthersRoot.id, true);
                    break;
			}

            item.OnCreated();

            _OnNextRepaint += () =>
            {
                var treeItem = GetItemByContent(item);
                if (treeItem != null)
                    BeginRename(treeItem);
            };

            Repaint();
        }
        #endregion

        #region Copy&Paste&Move&Delete
		void Delete(InputMapContentMapItem item)
        {
			Group.RemoveItem(item.Item);
			window.SetMapDirty();
			window.SelectInInspector(null);
			SetSelection(new List<int>());
			Reload();
        }

		//Drag & drop
		protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
		{
			DragAndDrop.PrepareStartDrag();
			DragAndDrop.SetGenericData("itemID", args.draggedItemIDs[0]);
			DragAndDrop.SetGenericData("tree", this);
			DragAndDrop.StartDrag(FindItem(args.draggedItemIDs[0], rootItem).displayName);
		}

		protected override bool CanStartDrag(CanStartDragArgs args)
		{
			if (!(args.draggedItem is InputMapContentMapItem item))
				return false;

			if (_isSearching)
				return false;

			return item.CanDrag;
		}

		protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
		{
			if (!(DragAndDrop.GetGenericData("tree") is InputMapWindowContentTree sourceTree) ||
				sourceTree != this)
				return DragAndDropVisualMode.Rejected;

			if (_isSearching)
				return DragAndDropVisualMode.Rejected;

			int itemID = (int)DragAndDrop.GetGenericData("itemID");

			if (!(FindItem(itemID, rootItem) is InputMapContentMapItem item))
				return DragAndDropVisualMode.Rejected;

			bool isBinding = item.Item is InputBinding;

			if (args.performDrop)
			{
				var targetItem = GetDraggedItemTarget(args);

				switch (targetItem)
				{
					case InputMapContentBindingHeader _:
						if (!isBinding)
							return DragAndDropVisualMode.Rejected;

						Move(Group.items.IndexOf(item.Item), Group.items.Count);
                        break;

                    case InputMapContentOtherHeader _:
                        if (isBinding)
                            return DragAndDropVisualMode.Rejected;

                        Move(Group.items.IndexOf(item.Item), Group.items.Count);
                        break;

					case InputMapContentMapItem targetMapItem:
						if (targetMapItem.Item is InputBinding != isBinding)
							return DragAndDropVisualMode.Rejected;

						Move(Group.items.IndexOf(item.Item), Group.items.IndexOf(targetMapItem.Item));

						break;
                }
			}

			Reload();
			return DragAndDropVisualMode.Move;
		}

		TreeViewItem GetDraggedItemTarget(DragAndDropArgs args)
		{
			if (!args.parentItem.children.IndexInRange(args.insertAtIndex))
				return args.parentItem;

			return args.parentItem.children[args.insertAtIndex];
		}

		public void Move(int oldIndex, int newIndex)
		{
			if (oldIndex == newIndex)
				return;

			var item = group.items[oldIndex];
			group.items[oldIndex] = null;
			group.items.Insert(newIndex, item);
			group.RemoveItem(newIndex < oldIndex ? oldIndex + 1 : oldIndex);
			SetSelection(new List<int>() { item.Guid.GetHashCode() });
			window.SetMapDirty();
		}
		#endregion

		#region Finding
		InputMapContentMapItem GetItemByContent<t>(t item) where t : InputMapItem
        {
			IList<int> items = GetDescendantsThatHaveChildren(rootItem.id);

			var targets = items
				.Select(x => FindItem(x, rootItem))
				.Where(x => x is InputMapContentMapItem mapItem && mapItem?.Item == item)
				.Select(x => x as InputMapContentMapItem);

			return targets.Count() == 1 ? targets.First() : null;
        }
        #endregion

        #region Renaming
        protected override bool CanRename(TreeViewItem item)
        {
            return item is InputMapContentMapItem;
        }

        protected override void RenameEnded(RenameEndedArgs args)
		{
			if (!args.acceptedRename) return;
			if (string.IsNullOrWhiteSpace(args.newName)) return;

			var item = FindItem(args.itemID, rootItem) as InputMapContentMapItem;

            if (NonRepeatableChecker.ContainsKey(Group.items, args.newName)) return;
            item.Item.ItemName = args.newName;

			window.SetMapDirty();
			Reload();
		}
        #endregion

        #region GUI
        protected override void RowGUI(RowGUIArgs args)
		{
			bool repaint = Event.current.type == EventType.Repaint;

			if (_OnNextRepaint != null && repaint)
            {
				_OnNextRepaint?.Invoke();
				_OnNextRepaint = null;
            }

			if (_showContextOnNextRepaint && repaint)
            {
				ShowContextMenu();
				_showContextOnNextRepaint = false;
            }

			//Button (either plus or minus)
			Rect buttonRect = new Rect(args.rowRect).ResizeToRight(GetCustomRowHeight(args.row, args.item));

			GUIContent buttonContent = new GUIContent();

			switch (args.item)
            {
				case InputMapContentMapItem _:
					buttonContent.image = qGUIEditorUtility.MinusIcon;
					break;
				case InputMapContentOtherHeader _:
					buttonContent.image = qGUIEditorUtility.PlusIconMore;
					break;
				default:
					buttonContent.image = qGUIEditorUtility.PlusIcon;
					break;
            }

			if (GUI.Button(buttonRect, buttonContent, Styles.Label))
			{
				switch (args.item)
				{
					case InputMapContentMapItem item:
						Delete(item);
						break;
					default:
						switch (args.item.id)
						{
							case BINDINGS_ROOT_ID:
								AddItem<InputBinding>();
								break;
							default:
								DisplayAddNewItemMenu();
                                break;
						}
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

		void DisplayAddNewItemMenu()
		{
			var types = WindowUtility.GetOtherItemTypesWithNames();
			GenericMenu menu = new GenericMenu();

			foreach (var item in types)
				menu.AddItem(item.name, false, () => AddItem(item.type));

			menu.ShowAsContext();
        }

		//Centers foldout vertically
		void CalcFoldoutOffset(float height) =>
			customFoldoutYOffset = (height - 16f) / 2;

		protected override float GetCustomRowHeight(int row, TreeViewItem item)
		{
			switch (item)
			{
				case InputMapContentMapItem _:
					return 18f;
				default:
					return 22f;
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