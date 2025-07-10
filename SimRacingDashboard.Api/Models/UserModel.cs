using System;
using SimRacingDashboard.Api.Services;

namespace SimRacingDashboard.Api.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }

    /// <summary>
    /// Encrypts the user model using the provided encryption service.
    /// This method is used to securely store user information by encrypting sensitive fields.
    /// The encrypted fields include Name, Email, ProfilePictureUrl, and Bio.
    /// The CreatedAt, LastLoginAt, IsActive, and Id fields are not encrypted
    /// as they are not considered sensitive.
    /// The PasswordHash is not included in the encrypted model as it should be handled separately.
    /// </summary>
    /// <param name="encryptionService">
    /// An instance of EncryptionService used to perform the encryption.
    /// This service should be configured with a secure key and IV for encryption.
    /// </param>
    /// <returns>An instance of EncryptedUserModel containing the encrypted fields.</returns>
    public EncryptedUserModel Encrypt(EncryptionService encryptionService)
    {
        return new EncryptedUserModel
        {
            Id = Id,
            EncryptedName = encryptionService.Encrypt(Name),
            EncryptedEmail = encryptionService.Encrypt(Email),
            EncryptedDisplayName = encryptionService.Encrypt(DisplayName),
            CreatedAt = CreatedAt,
            LastLoginAt = LastLoginAt,
            IsActive = IsActive,
            EncryptedProfilePictureUrl = ProfilePictureUrl != null ? encryptionService.Encrypt(ProfilePictureUrl) : null,
            EncryptedBio = Bio != null ? encryptionService.Encrypt(Bio) : null,
        };
    }
}
