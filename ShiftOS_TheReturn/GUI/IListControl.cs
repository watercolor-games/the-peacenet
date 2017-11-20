using System;
namespace Plex.Engine
{
    public interface IListControl
    {
        void ClearItems();
        void AddItem(object item);
        void RemoveItem(object item);
        void RemoveItemAt(int index);

        int SelectedIndex { get; set; }
        object SelectedItem { get; }

        event Action SelectedItemChanged;
    }
}
