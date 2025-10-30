using JurisSeeker.Domain.Common;

namespace JurisSeeker.Domain.Entities;

/// <summary>
/// Пользователь
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Имя/ник
    /// </summary>
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// Почта
    /// </summary>
    public string Email { get; set; } = null!;
    
    /// <summary>
    /// Пароль хэшированный
    /// </summary>
    public string Password { get; set; } = null!;
}