﻿//HintName: NoGenerationBuilder.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/distantcam/pocoman
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

public partial class NoGenerationBuilder
{
	private global::System.Func<global::NoGeneration> _builder;
	public NoGenerationBuilder()
	{
		_builder = () => new()
		{
		};
	}
	public global::NoGeneration Build()
	{
		var build = _builder();
		return build;
	}
	public static implicit operator global::NoGeneration(NoGenerationBuilder builder) => builder.Build();
}
