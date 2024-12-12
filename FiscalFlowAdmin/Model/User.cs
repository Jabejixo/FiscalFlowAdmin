using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;
using FiscalFlowAdmin.Model.Attributes;

namespace FiscalFlowAdmin.Model;

 [Table("authentication_user")]
    [Index("Email", Name = "authentication_user_email_key", IsUnique = true)]
    [AddINotifyPropertyChangedInterface]
    public sealed partial class User : Base
    {
        [Column("password")]
        [StringLength(8)]
        [MinLength(6)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,8}$", ErrorMessage = "Пароль должен содержать от 6 до 8 символов, включая минимум одну заглавную букву, одну цифру и один специальный символ.")]
        [Display(Name = "Пароль")]
        [Order(2)]
        [Required(ErrorMessage = "Пароль обязателен.")]
        [DataType(DataType.Password)]
        [Placeholder("Введите ваш пароль")]
        [Tooltip("Пароль пользователя.")]
        public string Password { get; set; } = null!;

        [Column("last_login")]
        [FormIgnore]
        [Display(Name = "Последний вход")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Tooltip("Время последнего входа в систему.")]
        public DateTime? LastLogin { get; set; }

        [Column("is_superuser")]
        [Display(Name = "Суперпользователь")]
        [FormIgnore]
        [DataGridIgnore]
        [DefaultValue(false)]
        [Tooltip("Указывает, имеет ли пользователь привилегии суперпользователя.")]
        public bool IsSuperuser { get; set; }

        [Column("email")]
        [StringLength(254)]
        [Display(Name = "Адрес электронной почты")]
        [Placeholder("Введите ваш почту")]
        [Order(1)]
        [Required(ErrorMessage = "Адрес электронной почты обязателен.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Неверный формат электронной почты.")]
        [Tooltip("Адрес электронной почты пользователя.")]
        [Nullable(false)]
        [Blank(false)]
        public string Email { get; set; } = null!;

        [Column("first_name")]
        [StringLength(30)]
        [Display(Name = "Имя")]
        [Order(3)]
        [Tooltip("Имя пользователя.")]
        public string? FirstName { get; set; }

        [Column("last_name")]
        [StringLength(30)]
        [Display(Name = "Фамилия")]
        [Order(4)]
        [Tooltip("Фамилия пользователя.")]
        public string? LastName { get; set; }

        [Column("birthday")]
        [Display(Name = "День рождения")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Order(5)]
        [Tooltip("Дата рождения пользователя.")]
        public DateOnly? Birthday { get; set; }

        [Column("is_active")]
        [Display(Name = "Активен")]
        [Order(6)]
        [DefaultValue(true)]
        [Required(ErrorMessage = "Поле активен обязательно.")]
        [Tooltip("Указывает, активна ли учетная запись пользователя.")]
        public bool IsActive { get; set; }

        [Column("is_confirmed_email")]
        [Display(Name = "Электронная почта подтверждена")]
        [Order(7)]
        [Tooltip("Указывает, подтверждена ли электронная почта пользователя.")]
        public bool? IsConfirmedEmail { get; set; }

        [Column("is_staff")]
        [Display(Name = "Сотрудник")]
        [Order(8)]
        [Required(ErrorMessage = "Поле сотрудник обязательно.")]
        [Tooltip("Указывает, является ли пользователь сотрудником.")]
        public bool IsStaff { get; set; }

        // Removed IDataErrorInfo implementation
    }
