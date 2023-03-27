using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels
{
    public abstract class BaseItem : INotifyPropertyChanged
    {
        public IDocumentManager DocumentManager { get; set; }
        public BaseItem Parent { get; set; }

        public virtual void SetVisibility(bool value, bool notify = true)
        {
            IsVisible = value;
            if (notify) NotifyIsVisibilty();
        }

        public void NotifyIsVisibilty()
        {
            NotifyChange(nameof(IsVisible));
            NotifyChange(nameof(IsVisibleHandler));
        }

        public virtual bool ValiderEstVisible()
        {
            return IsVisible;
        }

        protected BaseItem()
        {
            IsVisible = true;
        }

        public bool IsVisible { get; private set; }

        public virtual bool IsVisibleHandler => ValiderEstVisible();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyChange([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}