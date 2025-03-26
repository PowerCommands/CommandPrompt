﻿using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
namespace PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
public record CommandLineInput(string Raw, string Identifier, string[] Arguments, string[] Quotes, IDictionary<string, string> Options) : ICommandLineInput;