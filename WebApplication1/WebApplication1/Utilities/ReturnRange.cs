namespace WebApi.Utilities;

public record ReturnRange<T>(int Skip, int Count, T[] Items);