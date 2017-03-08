namespace Assets.Scripts.Models.Commands {
    public class Command {
        public MoveType moveType;
        public int? heat;

        public Command(MoveType moveType, int? heat = null) {
            this.moveType = moveType;
            this.heat = heat;
        }

        public ICommand MakeICommand() {
            if (heat != null) {
                switch (moveType) {
                    case MoveType.Fire:
                        return new SetFireCommand(heat.Value);
                    case MoveType.Water:
                        return new AddWaterCommand(heat.Value);
                }
            }
            return new RemoveCommand();
        }
    }
}
