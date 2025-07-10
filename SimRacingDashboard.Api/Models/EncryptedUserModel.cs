using System;
using SimRacingDashboard.Api.Services;

namespace SimRacingDashboard.Api.Models;

public class EncryptedUserModel
{
    public Guid Id { get; set; }
    public string EncryptedName { get; set; }
    public string EncryptedDisplayName { get; set; }
    public string EncryptedEmail { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
    public string? EncryptedProfilePictureUrl { get; set; }
    public string? EncryptedBio { get; set; }
    public string PasswordHash { get; set; }

    /// <summary>
    /// Decrypts the encrypted user model using the provided encryption service.
    /// This method is used to retrieve the original user information from the encrypted fields.
    /// The decrypted fields include Name, Email, ProfilePictureUrl, and Bio.
    /// The CreatedAt, LastLoginAt, IsActive, and Id fields are not encrypted
    /// as they are not considered sensitive.
    /// The PasswordHash is not included in the decrypted model as it should be handled separately.
    /// </summary>
    /// <param name="encryptionService">
    /// An instance of EncryptionService used to perform the decryption.
    /// This service should be configured with a secure key and IV for decryption.
    /// </param>
    /// <returns>An instance of UserModel containing the decrypted fields.</returns>
    public UserModel Decrypt(EncryptionService encryptionService)
    {
        return new UserModel
        {
            Id = Id,
            Name = encryptionService.Decrypt(EncryptedName),
            Email = encryptionService.Decrypt(EncryptedEmail),
            DisplayName = encryptionService.Decrypt(EncryptedDisplayName),
            CreatedAt = CreatedAt,
            LastLoginAt = LastLoginAt,
            IsActive = IsActive,
            ProfilePictureUrl = EncryptedProfilePictureUrl != null ? encryptionService.Decrypt(EncryptedProfilePictureUrl) : null,
            Bio = EncryptedBio != null ? encryptionService.Decrypt(EncryptedBio) : null,
        };
    }

}
