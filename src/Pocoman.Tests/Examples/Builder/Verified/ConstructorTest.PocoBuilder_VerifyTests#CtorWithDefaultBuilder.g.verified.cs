﻿//HintName: CtorWithDefaultBuilder.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/distantcam/pocoman
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

public partial class CtorWithDefaultBuilder
{
	private global::System.Func<global::CtorWithDefault> _builder;
	public CtorWithDefaultBuilder()
	{
		_builder = () => new()
		{
		};
	}
	public CtorWithDefaultBuilder UsingConstructor()
	{
		_builder = () => new()
		{
		};
		return this;
	}
	public CtorWithDefaultBuilder UsingConstructor(int number)
	{
		_builder = () => new(number)
		{
		};
		return this;
	}
	public CtorWithDefaultBuilder UsingConstructor(string name)
	{
		_builder = () => new(name)
		{
		};
		return this;
	}
	public global::CtorWithDefault Build()
	{
		var build = _builder();
		return build;
	}
	public static implicit operator global::CtorWithDefault(CtorWithDefaultBuilder builder) => builder.Build();
}
