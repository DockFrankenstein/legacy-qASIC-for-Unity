using System.Collections.Generic;
using qASIC.InputManagement;
using qASIC.InputManagement.Map;
using qASIC.Tools;

namespace qASIC.Console.Commands
{
    public class GameConsoleInputListCommand : GameConsoleCommand
    {
        public override bool Active => GameConsoleController.GetConfig().inputListCommand;
        public override string CommandName { get; } = "inputlist";
        public override string Description { get; } = "lists all actions and axes";
        public override string Help { get; } = "lists all actions and axes";
        public override string[] Aliases { get; } = new string[] { "listinput" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;

            if (!InputManager.MapLoaded)
            {
                LogError("Input Map has not been assigned!");
                return;
            }

            InputMap map = InputManager.Map;

            TextTree tree = new TextTree(GameConsoleController.GetConfig().textTreeStyle);
            TextTreeItem list = new TextTreeItem(map.name);

            foreach (InputGroup group in map.Groups)
            {
                TextTreeItem groupItem = new TextTreeItem(group.groupName);
                TextTreeItem actionsListItem = new TextTreeItem("Actions");
                TextTreeItem axexListItem = new TextTreeItem("Axes");

                foreach (InputAction action in group.actions)
                    actionsListItem.Add(action.actionName);

                foreach (InputAxis axis in group.axes)
                {
                    TextTreeItem axisItem = new TextTreeItem(axis.axisName)
                    {
                        children = new List<TextTreeItem>()
                        {
                            new TextTreeItem($" +{axis.positiveAction}"),
                            new TextTreeItem($" -{axis.negativeAction}"),
                        },
                    };

                    axexListItem.Add(axisItem);
                }

                groupItem.Add(actionsListItem);
                groupItem.Add(axexListItem);
                list.Add(groupItem);
            }

            string log = $"\n{tree.GenerateTree(list)}";

            Log(log.ToString(), "info");
        }
    }
}