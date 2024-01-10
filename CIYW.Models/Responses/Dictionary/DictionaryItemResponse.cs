﻿namespace CIYW.Models.Responses.Dictionary;

public class DictionaryItemResponse<TId>
{
    public TId Id { get; set; }
    public string Name { get; set; }
    public string Hint { get; set; }
}