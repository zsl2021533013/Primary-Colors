using System;
using QFramework;
using Script.Model;
using UnityEngine;

namespace Script.Architecture
{
    public enum ColorType
    {
        Null,
        White,
        Black,
        Red,
        Yellow,
        Blue,
        Orange,
        Purple,
        Green
    }

    public enum ParticleType
    {
        JumpSmoke,
        LandSmoke,
        Land,
        Shake,
        Bounce
    }

    public enum TileType
    {
        Touchable,
        Changeable,
        Unchangeable,
        Spike
    }

    public class PrimaryColors : Architecture<PrimaryColors>
    {
        protected override void Init()
        {
            RegisterModel(new PlayerModel());
            RegisterModel(new TileModel());
            RegisterModel(new ParticleModel());
            RegisterModel(new TargetModel());
            RegisterModel(new CollectibleModel());
        }
    }

    public static class ColorTypeExtension
    {
        /// <summary>
        /// Override add operation in enum
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ColorType Add(this ColorType a, ColorType b)
        {
            if (a == ColorType.Black || b == ColorType.Black)
            {
                return ColorType.Black;
            }

            if (a == ColorType.White)
            {
                return b;
            }
            
            switch (a)
            {
                case ColorType.Red:
                    switch (b)
                    {
                        case ColorType.Yellow:
                            return ColorType.Orange;
                        case ColorType.Blue:
                            return ColorType.Purple;
                        case ColorType.Orange:
                            return ColorType.Orange;
                        case ColorType.Purple:
                            return ColorType.Purple;
                        case ColorType.Green:
                            return ColorType.Black;
                    }
                    break;
                
                case ColorType.Yellow:
                    switch (b)
                    {
                        case ColorType.Red:
                            return ColorType.Orange;
                        case ColorType.Blue:
                            return ColorType.Green;
                        case ColorType.Orange:
                            return ColorType.Orange;
                        case ColorType.Purple:
                            return ColorType.Black;
                        case ColorType.Green:
                            return ColorType.Green;
                    }
                    break;
                
                case ColorType.Blue:
                    switch (b)
                    {
                        case ColorType.Red:
                            return ColorType.Purple;
                        case ColorType.Yellow:
                            return ColorType.Green;
                        case ColorType.Orange:
                            return ColorType.Black;
                        case ColorType.Purple:
                            return ColorType.Purple;
                        case ColorType.Green:
                            return ColorType.Green;
                    }
                    break;
                
                case ColorType.Orange:
                    switch (b)
                    {
                        case ColorType.Blue:
                            return ColorType.Black;
                        case ColorType.Purple:
                            return ColorType.Black;
                        case ColorType.Green:
                            return ColorType.Black;
                    }
                    break;
                
                case ColorType.Purple:
                    switch (b)
                    {
                        case ColorType.Yellow:
                            return ColorType.Black;
                        case ColorType.Orange:
                            return ColorType.Black;
                        case ColorType.Green:
                            return ColorType.Black;
                    }
                    break;
                
                case ColorType.Green:
                    switch (b)
                    {
                        case ColorType.Red:
                            return ColorType.Black;
                        case ColorType.Orange:
                            return ColorType.Black;
                        case ColorType.Purple:
                            return ColorType.Black;
                    }
                    break;
            }
            
            return a;
        }
        
        /// <summary>
        /// Translate color type to color
        /// </summary>
        /// <param name="colorType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Color ColorType2Color(this ColorType colorType)
        {
            var tmp = colorType switch
            {
                ColorType.White => "#FFFFFF",
                ColorType.Black => "#414141",
                ColorType.Red => "#FF5F5F",
                ColorType.Yellow => "#FFE960",
                ColorType.Blue => "#6075FF",
                ColorType.Orange => "#FFA960",
                ColorType.Purple => "#F67CFF",
                ColorType.Green => "#60FF60",
                _ => throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null)
            };
            
            ColorUtility.TryParseHtmlString(tmp, out var color);

            return color;
        }
    }
}
