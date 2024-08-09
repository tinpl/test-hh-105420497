namespace WebApi.Controllers;

public record ReturnRange<T>(int Skip, int Count, T[] Items);