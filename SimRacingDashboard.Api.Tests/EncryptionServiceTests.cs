using System;
using SimRacingDashboard.Api.Services;

namespace SimRacingDashboard.Api.Tests;

public class EncryptionServiceTests
{
    private readonly string key;
    private readonly string iv;

    public EncryptionServiceTests()
    {
        (key, iv) = EncryptionService.GenerateKeyAndIV();
    }

    [Fact]
    public void EncryptString_ShouldReturnEncryptedString()
    {
        // Arrange
        var encryptionService = new EncryptionService(key, iv);
        var originalString = "Hello, World!";

        // Act
        var encryptedString = encryptionService.Encrypt(originalString);

        // Assert
        Assert.NotNull(encryptedString);
        Assert.NotEqual(originalString, encryptedString);
    }

    [Fact]
    public void EncryptDecryptString_ShouldHandleGracefuly()
    {
        // Arrange
        var encryptionService = new EncryptionService(key, iv);
        var originalString = "Hello, World!";

        // Act
        var encryptedString = encryptionService.Encrypt(originalString);

        // Assert
        Assert.NotNull(encryptedString);
        Assert.NotEqual(originalString, encryptedString);

        var decryptedString = encryptionService.Decrypt(encryptedString);
        Assert.NotNull(decryptedString);
        Assert.Equal(originalString, decryptedString);
    }

    [Fact]
    public void Decrypt_EmptyString_ShouldReturnEmptyString()
    {
        // Arrange
        var encryptionService = new EncryptionService(key, iv);
        var encryptedString = encryptionService.Encrypt(string.Empty);

        // Act
        var decryptedString = encryptionService.Decrypt(encryptedString);

        // Assert
        Assert.NotNull(decryptedString);
        Assert.Equal(string.Empty, decryptedString);
    }

    [Fact]
    public void Decrypt_NullString_ShouldThrowArgumentNullException()
    {
        // Arrange
        var encryptionService = new EncryptionService(key, iv);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => encryptionService.Decrypt(null));
    }
}
