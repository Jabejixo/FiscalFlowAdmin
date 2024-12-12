using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[AddINotifyPropertyChangedInterface]
public abstract class Base : INotifyPropertyChanged
{
    [Key]
    [Column("id")]
    public virtual long Id { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}