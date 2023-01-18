using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace CLI
{
    /// <summary>
    /// Base class for implementing commands in an object-oriented and modular way.
    /// Register with a <see cref="CommandProcessor"/> to add to the command line interface.
    /// </summary>
    abstract public class Command
    {
        /// <summary>
        /// Gets the name of the <see cref="Command"/>.
        /// Lowercase version is used as the command line keyword.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the description of the <see cref="Command"/>.
        /// Output to describe the <see cref="Command"/> when called from the command line.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the author of the <see cref="Command"/>.
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// Gets the version of the <see cref="Command"/>.
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// Gets the <see cref="Argument"/>s the <see cref="Command"/> takes.
        /// <example>
        /// bake <cake/bread/muffin> <amount>
        /// </example>
        /// </summary>
        public virtual List<Argument> Arguments => new();

        /// <summary>
        /// Gets the <see cref="Option"/>s the <see cref="Command"/> takes.
        /// <example>
        /// bake <type> [options]
        /// bake cake --noLies --darkChocolate -v
        /// </example>
        /// </summary>
        public virtual List<Option> Options => new();

        /// <summary>
        /// Gets the validators the <see cref="Command"/> uses.
        /// </summary>
        public virtual List<ValidateSymbolResult<CommandResult>> Validators => new();

        /// <summary>
        /// Gets the aliases of the <see cref="Command"/>.
        /// </summary>
        public virtual List<string> Aliases => new();

        /// <summary>
        /// The internal backing field for <see cref="Context"/>.
        /// </summary>
        protected InvocationContext? _context;

        /// <summary>
        /// Gets the <see cref="InvocationContext"/> if it has been passed to the command upon <see cref="Execute"/>.
        /// Returns <see cref="null"/> if the <see cref="Command"/> has not yet been run by the <see cref="CommandProcessor"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="Command"/> has not yet had its
        /// <see cref="InvocationContext"/> initialized.</exception>
        public InvocationContext Context
        {
            get
            {
                if (_context == null)
                {
                    throw new InvalidOperationException($"Command '{Name}' context can only be accessed after the command is initialized by having " +
                        $"its Execute method called by the CommandProcessor.");
                }

                return _context;
            }
        }

        /// <summary>
        /// Executes the <see cref="Command"/> with its arguments and options.
        /// </summary>
        /// <param name="context"></param>
        public void Execute(InvocationContext context)
        {
            _context = context;
            InternalExecute();
        }

        /// <summary>
        /// The internal implementation run after <see cref="Command"/> initialization by <see cref="Execute"/>.
        /// </summary>
        protected abstract void InternalExecute();

        /// <summary>
        /// Parses the <paramref name="argument"/> from the <see cref="_context"/>.
        /// </summary>
        /// <param name="argument">The <see cref="Argument"/> to parse from the <see cref="_context"/>.</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"> Thrown if the provided <paramref name="argument"/> could not be found in the
        /// <see cref="_context"/>.</exception>
        public object Parse(Argument argument)
        {
            var value = Context.ParseResult.GetValueForArgument(argument);

            if (value == null)
            {
                throw new KeyNotFoundException($"The argument '{argument.Name}' could not be found.");
            }

            return value;
        }

        /// <summary>
        /// Parses the <paramref name="option"/> from the <paramref name="context"/>.
        /// </summary>
        /// <param name="option">The <see cref="Option"/> to parse from the <see cref="_context"/>.</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"> Thrown if the provided <paramref name="option"/> could not be found in the
        /// <see cref="_context"/>.</exception>
        public object Parse(Option option)
        {
            var value = Context.ParseResult.GetValueForOption(option);

            if (value == null)
            {
                throw new KeyNotFoundException($"The option '{option.Name}' could not be found.");
            }

            return value;
        }
    }
}

