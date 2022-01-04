using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace qASIC.InputManagement.Internal
{
    public class InputMapContentTree : TreeView
    {
		public InputGroup Group { get; set; }

		public InputMapContentTree(TreeViewState state)
			: base(state)
		{
			Reload();
		}

		protected override TreeViewItem BuildRoot()
		{
			return new TreeViewItem { id = 0, depth = -1 };
		}

		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			var rows = GetRows() ?? new List<TreeViewItem>();

			Scene scene = SceneManager.GetSceneAt(0);

			// We use the GameObject instanceIDs as ids for items as we want to 
			// select the game objects and not the transform components.
			rows.Clear();

			Debug.Log(Group == null);

			if (Group != null)
			{
				for (int i = 0; i < Group.actions.Count; i++)
				{
					TreeViewItem item = new TreeViewItem(i + 1, -1, Group.actions[i].actionName);
					root.AddChild(item);
					rows.Add(item);
				}
			}

            SetupDepthsFromParentsAndChildren(root);
			return rows;
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			//Event evt = Event.current;
			//extraSpaceBeforeIconAndLabel = 18f;


			//var gameObject = GetGameObject(args.item.id);
			//if (gameObject == null)
			//	return;

			//Rect toggleRect = args.rowRect;
			//toggleRect.x += GetContentIndent(args.item);
			//toggleRect.width = 16f;

			//if (evt.type == EventType.MouseDown && toggleRect.Contains(evt.mousePosition))
			//	SelectionClick(args.item, false);

			//EditorGUI.BeginChangeCheck();
			//bool isStatic = EditorGUI.Toggle(toggleRect, gameObject.isStatic);
			//if (EditorGUI.EndChangeCheck())
			//	gameObject.isStatic = isStatic;

			base.RowGUI(args);
		}
	}
}