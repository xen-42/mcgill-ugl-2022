using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Utils
{
    public static Sprite LoadAvatar(int imageID)
    {
        if (imageID != -1)
        {
            if (SteamUtils.GetImageSize(imageID, out uint width, out uint height))
            {
                var size = width * height * 4;
                byte[] image = new byte[size];
                if (SteamUtils.GetImageRGBA(imageID, image, (int)size))
                {
                    var texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                    texture.LoadRawTextureData(image);
                    texture.Apply();
                    return Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(texture.width / 2f, texture.height / 2f)
                        );
                }
            }
        }

        return null;
    }
}
