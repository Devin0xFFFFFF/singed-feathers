namespace Assets.Scripts.Models.Commands {
    public class Command {
        public MoveType _moveType;
        public int _heat;

        public Command(MoveType moveType, int heat) {
            _moveType = moveType;
            _heat = heat;
        }

        public ICommand MakeICommand() {
            switch (_moveType) {
                case MoveType.Fire:
                    return new SetFireCommand(_heat);
                case MoveType.Water:
                    return new AddWaterCommand(_heat);
                default:
                    return new RemoveCommand();
            }
        }
    }
}
