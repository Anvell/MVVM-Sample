namespace MVVMSample.Interfaces
{
    interface IBasicCommands {
        void OnNewCommand();
        void OnDeleteCommand();
        void OnCopyCommand();
        void OnPasteCommand();
        void OnUndoCommand();
    }
}
