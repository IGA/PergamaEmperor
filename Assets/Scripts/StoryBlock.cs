namespace PergamaApp
{
    public class StoryBlock
    {
        public int Id { get; set; }
        public string StoryText { get; set; }
        public StoryOption[] Options { get; set; }
    }
}