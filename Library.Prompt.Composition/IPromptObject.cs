namespace Library.Prompt.Composition;

public interface IPromptObject<T> where T : IPromptObject<T>
{
    static abstract T Example { get; }
}