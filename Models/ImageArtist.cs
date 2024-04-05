namespace AnimePicturesScroller.Models
{
	public class ImageArtist
	{
		public long Id { get; set; }

		public Guid Id_V2 { get; set; }

		public string Name { get; set; }

		public List<string> Aliases { get; set; }

		public object? Image_Url { get; set; }

		public Uri[] Links { get; set; }

		public object Policy_Repost { get; set; }

		public bool Policy_Credit { get; set; }

		public bool Policy_Ai { get; set; }
	}
}
