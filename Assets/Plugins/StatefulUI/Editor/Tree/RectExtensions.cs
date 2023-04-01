using System;
using System.Linq;
using UnityEngine;

namespace StatefulUI.Editor.Tree
{
    public static class RectExtensions
    {
        public static Rect Abs(this Rect rect)
        {
            if (rect.width < 0)
            {
                rect.x += rect.width;
                rect.width *= -1;
            }

            if (rect.height < 0)
            {
                rect.y += rect.height;
                rect.height *= -1;
            }

            return rect;
        }

        public static Rect Invert(this Rect rect)
        {
            return new Rect( rect.y, rect.x, rect.height, rect.width );
        }

        public static Rect Union(this Rect rect, params Rect[] other)
        {
            if (other == null || other.Length == 0)
            {
                return rect;
            }

            if (other.Length == 1 && other[0] == rect)
            {
                return rect;
            }
            var xMin = Math.Min(rect.xMin, other.Select(x => x.xMin).Aggregate(Math.Min));
            var yMin = Math.Min(rect.yMin, other.Select(x => x.yMin).Aggregate(Math.Min));
            var xMax = Math.Max(rect.xMax, other.Select(x => x.xMax).Aggregate(Math.Max));
            var yMax = Math.Max(rect.yMax, other.Select(x => x.yMax).Aggregate(Math.Max));
            
            return Rect.MinMaxRect( xMin, xmax: xMax, ymin: yMin, ymax: yMax );
        }

        public static Rect Intend(this Rect rect, float border)
        {
            rect = rect.Abs();

            var result = new Rect( rect.x + border, rect.y + border, rect.width - 2 * border, rect.height - 2 * border );

            if (result.width < 0)
            {
                result.x += result.width / 2;
                result.width = 0;
            }

            if (result.height < 0)
            {
                result.y += result.height / 2;
                result.height = 0;
            }

            return result;
        }

        public static Rect Extend(this Rect rect, float border)
        {
            rect = rect.Abs();
            return new Rect( rect.x - border, rect.y - border, rect.width + 2 * border, rect.height + 2 * border );
        }

        public static Rect FirstLine(this Rect rect, float height = 18)
        {
            rect = rect.Abs();
            rect.height = height;
            return rect.Abs();
        }

        public static Vector2 TopLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMin);
        }

        public static Rect ScaleSizeBy(this Rect rect, float scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }

        public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
        {
            var result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            
            return result;
        }

        public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }

        public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
        {
            var result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale.x;
            result.xMax *= scale.x;
            result.yMin *= scale.y;
            result.yMax *= scale.y;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            
            return result;
        }
    }
}
