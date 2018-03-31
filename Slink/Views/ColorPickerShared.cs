using System;
using System.Collections.Generic;
using Plugin.DeviceInfo;

namespace Slink
{

    public class ColorPickerShared
    {
        List<Model> CollectionItems;

        public List<Model> GetCollectionItems()
        {
            List<int[]> Colors = new List<int[]>()
            {
                new int[] { 255, 170, 170, 255 },
                new int[] { 255, 86, 86, 255 },
                new int[] { 255, 0, 0, 255 },
                new int[] { 191, 0, 0, 255 },
                new int[] { 127, 0, 0, 255 },
                new int[] { 255, 255, 255, 255 },
                new int[] { 255, 212, 170, 255 },
                new int[] { 255, 170, 86, 255 },
                new int[] { 255, 127, 0, 255 },
                new int[] { 191, 95, 0, 255 },
                new int[] { 127, 63, 0, 255 },
                new int[] { 229, 229, 229, 255 },
                new int[] { 255, 255, 170, 255 },
                new int[] { 255, 255, 86, 255 },
                new int[] { 255, 255, 0, 255 },
                new int[] { 191, 191, 0, 255 },
                new int[] { 127, 127, 0, 255 },
                new int[] { 204, 204, 204, 255 },
                new int[] { 212, 255, 170, 255 },
                new int[] { 170, 255, 86, 255 },
                new int[] { 127, 255, 0, 255 },
                new int[] { 95, 191, 0, 255 },
                new int[] { 63, 127, 0, 255 },
                new int[] { 178, 178, 178, 255 },
                new int[] { 170, 255, 170, 255 },
                new int[] { 86, 255, 86, 255 },
                new int[] { 0, 255, 0, 255 },
                new int[] { 0, 191, 0, 255 },
                new int[] { 0, 127, 0, 255 },
                new int[] { 153, 153, 153, 255 },
                new int[] { 170, 255, 212, 255 },
                new int[] { 86, 255, 170, 255 },
                new int[] { 0, 255, 127, 255 },
                new int[] { 0, 191, 95, 255 },
                new int[] { 0, 127, 63, 255 },
                new int[] { 127, 127, 127, 255 },
                new int[] { 170, 255, 255, 255 },
                new int[] { 86, 255, 255, 255 },
                new int[] { 0, 255, 255, 255 },
                new int[] { 0, 191, 191, 255 },
                new int[] { 0, 127, 127, 255 },
                new int[] { 102, 102, 102, 255 },
                new int[] { 170, 212, 255, 255 },
                new int[] { 86, 170, 255, 255 },
                new int[] { 0, 127, 255, 255 },
                new int[] { 0, 95, 191, 255 },
                new int[] { 0, 63, 127, 255 },
                new int[] { 76, 76, 76, 255 },
                new int[] { 170, 170, 255, 255 },
                new int[] { 86, 86, 255, 255 },
                new int[] { 0, 0, 255, 255 },
                new int[] { 0, 0, 191, 255 },
                new int[] { 0, 0, 127, 255 },
                new int[] { 51, 51, 51, 255 },
                new int[] { 212, 170, 255, 255 },
                new int[] { 170, 86, 255, 255 },
                new int[] { 127, 0, 255, 255 },
                new int[] { 95, 0, 191, 255 },
                new int[] { 63, 0, 127, 255 },
                new int[] { 25, 25, 25, 255 },
                new int[] { 255, 170, 255, 255 },
                new int[] { 255, 86, 255, 255 },
                new int[] { 255, 0, 255, 255 },
                new int[] { 191, 0, 191, 255 },
                new int[] { 127, 0, 127, 255 },
                new int[] { 10, 10, 0, 255 },
                new int[] { 255, 170, 212, 255 },
                new int[] { 255, 86, 170, 255 },
                new int[] { 255, 0, 127, 255 },
                new int[] { 191, 0, 95, 255 },
                new int[] { 127, 0, 63, 255 },
                new int[] { 0, 0, 0, 255 }
            };

            CollectionItems = new List<Model>();

            foreach (int[] color in Colors)
            {
                var item = new Model();
                item.color = color;
                CollectionItems.Add(item);
            }

            //DataStructure Pivot required to render correctly in Android
            if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.Android)
            {
                int starting = 0;
                int index = 0;
                var newItems = new List<ColorPickerShared.Model>();
                while (newItems.Count != CollectionItems.Count)
                {
                    var target = CollectionItems[index];
                    newItems.Add(target);

                    index += 6; //num rows

                    if (index >= CollectionItems.Count)
                    {
                        starting++;
                        index = starting;
                    }
                }
                CollectionItems = newItems;
            }

            return CollectionItems;
        }
        public class Model
        {
            public int[] color;
            public bool animationCompleted;
        }
    }
}
