namespace Micro.Net.Example.Services
{
    public interface IValueHolderService
    {
        void ShiftValue(int amount);
        int ReadValue();
    }
}