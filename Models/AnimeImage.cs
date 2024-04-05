namespace AnimePicturesScroller.Models
{
	public class AnimeImage
	{
		public long Id { get; set; }

		public Guid Id_V2 { get; set; }

		public Uri Image_Url { get; set; }

		public Uri Sample_Url { get; set; }

		public long Image_Size { get; set; }

		public long Image_Width { get; set; }

		public long Image_Height { get; set; }

		public long Sample_Size { get; set; }

		public long Sample_Width { get; set; }

		public long Sample_Height { get; set; }

		public Uri Source { get; set; }

		public object Source_Id { get; set; }

		public string Rating { get; set; }

		public string Verification { get; set; }

		public string Hash_Md5 { get; set; }

		public string Hash_Perceptual { get; set; }

		public List<int> Color_Dominant { get; set; }

		public List<List<int>> Color_Palette { get; set; }

		public object? Duration { get; set; }

		public bool Is_Original { get; set; }

		public bool Is_Screenshot { get; set; }

		public bool Is_Flagged { get; set; }

		public bool Is_Animated { get; set; }

		public ImageArtist? Artist { get; set; }

		public List<ImageCharacter>? Characters { get; set; }

		public List<object>? Tags { get; set; }

		public double Created_At { get; set; }

		public double Updated_At { get; set; }
	}
}
