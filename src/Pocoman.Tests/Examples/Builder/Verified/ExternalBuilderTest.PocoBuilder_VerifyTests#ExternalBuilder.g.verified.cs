﻿//HintName: ExternalBuilder.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/distantcam/pocoman
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

partial class ExternalBuilder
{
	private global::System.Func<global::External> _builder;
	public ExternalBuilder()
	{
		_builder = () => new()
		{
			IsRequired = _isRequired_isSet ? _isRequired : throw new global::System.InvalidOperationException("Property \"IsRequired\" (string) must be set before build can be called."),
			StringInit = _stringInit_isSet ? _stringInit : default,
			NumberInit = _numberInit_isSet ? _numberInit : default
		};
	}
	private bool _isRequired_isSet;
	private string _isRequired = default;
	public ExternalBuilder WithIsRequired(string value)
	{
		_isRequired = value;
		_isRequired_isSet = true;
		return this;
	}
	private bool _stringInit_isSet;
	private string _stringInit = default;
	public ExternalBuilder WithStringInit(string value)
	{
		_stringInit = value;
		_stringInit_isSet = true;
		return this;
	}
	private bool _numberInit_isSet;
	private int _numberInit = default;
	public ExternalBuilder WithNumberInit(int value)
	{
		_numberInit = value;
		_numberInit_isSet = true;
		return this;
	}
	private bool _standard_isSet;
	private string _standard = default;
	public ExternalBuilder WithStandard(string value)
	{
		_standard = value;
		_standard_isSet = true;
		return this;
	}
	public global::External Build()
	{
		var build = _builder();
		if (_standard_isSet)
			build.Standard = _standard;
		return build;
	}
	public static implicit operator global::External(ExternalBuilder builder) => builder.Build();
}
