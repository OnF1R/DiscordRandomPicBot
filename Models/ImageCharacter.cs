namespace AnimePicturesScroller.Models
{
	public class ImageCharacter
	{
		public long Id { get; set; }

		public Guid Id_V2 { get; set; }

		public string Name { get; set; }

		public List<string>? Aliases { get; set; }

		public string Description { get; set; }

		public List<int>? Ages { get; set; }

		public int? Height { get; set; }

		public int? Weight { get; set; }

		public string Gender { get; set; }

		public string Species { get; set; }

		public object? Birthday { get; set; }

		public string Nationality { get; set; }

		public List<string> Occupations { get; set; }

		public long? Main_Image_Id { get; set; }
	}
}
