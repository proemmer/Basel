using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace BandSlider.Tile
{
	internal class BandSliderTileLayout
	{
		private readonly PageLayout pageLayout;
		private readonly PageLayoutData pageLayoutData;
		
		private readonly FlowPanel panel = new FlowPanel();
		internal TextButton StartStop = new TextButton();
		internal TextButton StartStopDetection = new TextButton();
		internal TextButton Next = new TextButton();
		internal TextButton Prev = new TextButton();
		
		internal TextButtonData StartStopData = new TextButtonData(2, "Start");
		internal TextButtonData StartStopDetectionData = new TextButtonData(3, "Detect");
		internal TextButtonData NextData = new TextButtonData(4, "Next");
		internal TextButtonData PrevData = new TextButtonData(5, "Prev");
		
		public BandSliderTileLayout()
		{
			LoadIconMethod = LoadIcon;
			AdjustUriMethod = (uri) => uri;
			
			panel = new FlowPanel();
			panel.Orientation = FlowPanelOrientation.Vertical;
			panel.Rect = new PageRect(0, 0, 257, 128);
			panel.ElementId = 1;
			panel.Margins = new Margins(0, 0, 0, 0);
			panel.HorizontalAlignment = HorizontalAlignment.Left;
			panel.VerticalAlignment = VerticalAlignment.Top;
			
			StartStop = new TextButton();
			StartStop.PressedColor = new BandColor(32, 32, 32);
			StartStop.Rect = new PageRect(0, 0, 125, 60);
			StartStop.ElementId = 2;
			StartStop.Margins = new Margins(0, 2, 0, 0);
			StartStop.HorizontalAlignment = HorizontalAlignment.Center;
			StartStop.VerticalAlignment = VerticalAlignment.Top;
			
			panel.Elements.Add(StartStop);
			
			StartStopDetection = new TextButton();
			StartStopDetection.PressedColor = new BandColor(32, 32, 32);
			StartStopDetection.Rect = new PageRect(0, 0, 125, 60);
			StartStopDetection.ElementId = 3;
			StartStopDetection.Margins = new Margins(130, -60, 0, 0);
			StartStopDetection.HorizontalAlignment = HorizontalAlignment.Center;
			StartStopDetection.VerticalAlignment = VerticalAlignment.Top;
			
			panel.Elements.Add(StartStopDetection);
			
			Next = new TextButton();
			Next.PressedColor = new BandColor(32, 32, 32);
			Next.Rect = new PageRect(0, 0, 125, 60);
			Next.ElementId = 4;
			Next.Margins = new Margins(130, 4, 0, 0);
			Next.HorizontalAlignment = HorizontalAlignment.Center;
			Next.VerticalAlignment = VerticalAlignment.Top;
			
			panel.Elements.Add(Next);
			
			Prev = new TextButton();
			Prev.PressedColor = new BandColor(32, 32, 32);
			Prev.Rect = new PageRect(0, 0, 125, 60);
			Prev.ElementId = 5;
			Prev.Margins = new Margins(0, -60, 0, 0);
			Prev.HorizontalAlignment = HorizontalAlignment.Center;
			Prev.VerticalAlignment = VerticalAlignment.Top;
			
			panel.Elements.Add(Prev);
			pageLayout = new PageLayout(panel);
			
			PageElementData[] pageElementDataArray = new PageElementData[4];
			pageElementDataArray[0] = StartStopData;
			pageElementDataArray[1] = StartStopDetectionData;
			pageElementDataArray[2] = NextData;
			pageElementDataArray[3] = PrevData;
			
			pageLayoutData = new PageLayoutData(pageElementDataArray);
		}
		
		public PageLayout Layout
		{
			get
			{
				return pageLayout;
			}
		}
		
		public PageLayoutData Data
		{
			get
			{
				return pageLayoutData;
			}
		}
		
		public Func<string, Task<BandIcon>> LoadIconMethod
		{
			get;
			set;
		}
		
		public Func<string, string> AdjustUriMethod
		{
			get;
			set;
		}
		
		private static async Task<BandIcon> LoadIcon(string uri)
		{
			StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));
			
			using (IRandomAccessStream fileStream = await imageFile.OpenAsync(FileAccessMode.Read))
			{
				WriteableBitmap bitmap = new WriteableBitmap(1, 1);
				await bitmap.SetSourceAsync(fileStream);
				return bitmap.ToBandIcon();
			}
		}
		
		public async Task LoadIconsAsync(BandTile tile)
		{
			await Task.Run(() => { }); // Dealing with CS1998
		}
		
		public static BandTheme GetBandTheme()
		{
			var theme = new BandTheme();
			theme.Base = new BandColor(51, 102, 204);
			theme.HighContrast = new BandColor(58, 120, 221);
			theme.Highlight = new BandColor(58, 120, 221);
			theme.Lowlight = new BandColor(49, 101, 186);
			theme.Muted = new BandColor(43, 90, 165);
			theme.SecondaryText = new BandColor(137, 151, 171);
			return theme;
		}
		
		public static BandTheme GetTileTheme()
		{
			var theme = new BandTheme();
			theme.Base = new BandColor(51, 102, 204);
			theme.HighContrast = new BandColor(58, 120, 221);
			theme.Highlight = new BandColor(58, 120, 221);
			theme.Lowlight = new BandColor(49, 101, 186);
			theme.Muted = new BandColor(43, 90, 165);
			theme.SecondaryText = new BandColor(137, 151, 171);
			return theme;
		}
		
		public class PageLayoutData
		{
			private readonly PageElementData[] array;
			
			public PageLayoutData(PageElementData[] pageElementDataArray)
			{
				array = pageElementDataArray;
			}
			
			public int Count
			{
				get
				{
					return array.Length;
				}
			}
			
			public T Get<T>(int i) where T : PageElementData
			{
				return (T)array[i];
			}
			
			public T ById<T>(short id) where T:PageElementData
			{
				return (T)array.FirstOrDefault(elm => elm.ElementId == id);
			}
			
			public PageElementData[] All
			{
				get
				{
					return array;
				}
			}
		}
		
	}
}
