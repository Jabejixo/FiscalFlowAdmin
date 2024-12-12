using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Runtime.CompilerServices;
using FiscalFlowAdmin.Model.Attributes;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[AddINotifyPropertyChangedInterface]
public abstract class Base : INotifyPropertyChanged, INotifyDataErrorInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [FormIgnore]
    [DataGridIgnore]
    [Column("id")]
    public virtual long Id { get; set; }
    
       // Реализация INotifyDataErrorInfo
        private readonly ConcurrentDictionary<string, List<string>> _errors = new ConcurrentDictionary<string, List<string>>();
        
        [FormIgnore]
        [DataGridIgnore]
        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return _errors.SelectMany(err => err.Value);
            if (_errors.TryGetValue(propertyName, out var errors))
                return errors;
            return Enumerable.Empty<string>();
        }

        public void ValidateProperty(string propertyName, object? value)
        {
            var validationContext = new ValidationContext(this)
            {
                MemberName = propertyName
            };
            var validationResults = new List<ValidationResult>();

            // Очистка предыдущих ошибок
            _errors.TryRemove(propertyName, out _);

            // Валидация свойства
            bool isValid = Validator.TryValidateProperty(value, validationContext, validationResults);

            if (!isValid)
            {
                _errors[propertyName] = validationResults.Select(r => r.ErrorMessage!).ToList();
            }

            // Вызов события ErrorsChanged
            OnErrorsChanged(propertyName);
        }

        public void ValidateAllProperties()
        {
            var properties = this.GetType().GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.GetCustomAttributes<ValidationAttribute>().Any());

            foreach (var property in properties)
            {
                var value = property.GetValue(this);
                ValidateProperty(property.Name, value);
            }
        }

        protected virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        // Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
}