using System;
using System.Threading.Tasks;
using Godot;

namespace bigdex.scripts.API;

public static class ImageCreator
{
    /// <summary>
    /// The General Image size of the WIDTH and HEIGHT.
    /// </summary>
    private const int GeneralImageSize = 64;
    
    /// <summary>
    /// Returns the general image size used by the game's nikos.
    /// </summary>
    /// <returns></returns>
    private static Vector2I GetGeneralImageSize()
    {
        return new Vector2I(GeneralImageSize, GeneralImageSize);
    }
    
    /// <summary>
    /// The result of the image get call.
    /// </summary>
    /// <param name="texture">The texture itself.</param>
    /// <param name="isLoaded">Is the texture loaded?</param>
    public struct ImageResult(ImageTexture texture, bool isLoaded)
    {
        public readonly ImageTexture Texture = texture;
        public readonly bool IsLoaded = isLoaded;
    }
    
    public static async Task<ImageResult> CreateImageTextureFromUrl(string url)
    {
        GD.Print($"Downloading image: {url} and resizing to {GetGeneralImageSize()}");
        try
        {
            var byteArray = await NikoDexApi.RequestFromApiBytes(url);
            var image = new Image();

            if (image.LoadPngFromBuffer(byteArray) != Error.Ok)
                throw new BadImageFormatException("Cannot load image");
            
            var imageTexture = ImageTexture.CreateFromImage(image);
            
            // One command to resize an image.
            // GameMaker could probably learn SOMETHING from this!
            imageTexture.SetSizeOverride(GetGeneralImageSize());
            
            return new ImageResult(imageTexture, true);
        }
        catch (Exception)
        {
            return new ImageResult(null, false);
        }
        
    }

    /// <summary>
    /// Creates a Niko image from a buffer.
    /// </summary>
    /// <param name="buffer">Buffer.</param>
    /// <returns>The created ImageTexture.</returns>
    /// <exception cref="BadImageFormatException">If the image is not formatted appropriately, this exception will be thrown.</exception>
    public static ImageTexture CreateImageTextureFromByteBuffer(byte[] buffer)
    {
        var image = new Image();

        if (image.LoadPngFromBuffer(buffer) != Error.Ok)
            throw new BadImageFormatException("Cannot load image");

        var imageTexture = ImageTexture.CreateFromImage(image);
        imageTexture.SetSizeOverride(GetGeneralImageSize());
        return imageTexture;
    }
}