using System.CommandLine;

namespace CLI
{
    /// <summary>
    /// Processes <see cref="Command"/> that are registered with <see cref="Register"/>
    /// Adds the command to the command line interface. Handles basic argument and option validation.
    /// Handles all usage, help, and command line interface menus.
    /// </summary>
    public class CommandProcessor
    {
        /// <summary>
        /// Arguments received from the command line interface. 
        /// </summary>
        protected readonly string[] _arguments;

        /// <summary>
        /// <see cref="RootCommand"/> used as the primary command line interface.
        /// </summary>
        protected readonly RootCommand _rootCommand;

        /// <summary>
        /// Constructs a new <see cref="CommandProcessor"/> with the provided <paramref name="arguments"/> and
        /// optional <paramref name="description"/>.
        /// </summary>
        /// <param name="arguments">The argument received from the command line to use in processing.</param>
        /// <param name="description">If provided, the description shown in the primary command line interface.</param>
        public CommandProcessor(string[] arguments, string description = "")
        { 
            _arguments = arguments;
            _rootCommand = new RootCommand(description);
        }

        /// <summary>
        /// Registers a <see cref="Command"/> with the <see cref="CommandProcessor"/>. Adds a <see cref="Command"/>, its arguments,
        /// its options, and its validators to the command line interface.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> to register with the <see cref="CommandProcessor"/>.</param>
        public async void Register(Command command)
        {
            var addCommand = new System.CommandLine.Command(command.Name, command.Description);

            await Task.WhenAll(
                AddArguments(command, addCommand),
                AddOptions(command, addCommand),
                AddValidators(command, addCommand),
                AddAliases(command, addCommand)
            );

            addCommand.SetHandler((context) => command.Execute(context));

            _rootCommand.AddCommand(addCommand);
        }

        /// <summary>
        /// Adds the <paramref name="command"/>'s <see cref="Argument"/>s to the <paramref name="addCommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> with the <see cref="Argument"/>s to add to the
        /// <paramref name="addCommand"/>.</param>
        /// <param name="addCommand">The <see cref="System.CommandLine.Command"/> to add the <paramref name="command"/>
        /// <see cref="Argument"/>s to.</param>
        /// <returns>The <see cref="Task"/> for adding arguments.</returns>
        protected static Task AddArguments(Command command, System.CommandLine.Command addCommand)
        {
            foreach (var argument in command.Arguments)
            {
                addCommand.AddArgument(argument);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds the <paramref name="command"/>'s <see cref="Option"/>s to the <paramref name="addCommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> with the <see cref="Option"/>s to add to the
        /// <paramref name="addCommand"/>.</param>
        /// <param name="addCommand">The <see cref="System.CommandLine.Command"/> to add the <paramref name="command"/>
        /// <see cref="Option"/>s to.</param>
        /// <returns>The <see cref="Task"/> for adding options.</returns>
        protected static Task AddOptions(Command command, System.CommandLine.Command addCommand)
        {
            foreach (var option in command.Options)
            {
                addCommand.AddOption(option);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds the <paramref name="command"/>'s validators to the <paramref name="addCommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> with the validators to add to the
        /// <paramref name="addCommand"/>.</param>
        /// <param name="addCommand">The <see cref="System.CommandLine.Command"/> to add the <paramref name="command"/>
        /// validators to.</param>
        /// <returns>The <see cref="Task"/> for adding validators.</returns>
        protected static Task AddValidators(Command command, System.CommandLine.Command addCommand)
        {
            foreach (var validator in command.Validators)
            {
                addCommand.AddValidator(validator);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds the <paramref name="command"/>'s aliases to the <paramref name="addCommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> with the aliases to add the
        /// <paramref name="addCommand"/>.
        /// <param name="addCommand">The <see cref="System.CommandLine.Command"/> to add the <paramref name="command"/>
        /// aliases to.</param>
        /// <returns>The <see cref="Task"/> for adding aliases.</returns>
        protected static Task AddAliases(Command command, System.CommandLine.Command addCommand)
        {
            foreach (var alias in command.Aliases)
            {
                addCommand.AddAlias(alias);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers multiple <see cref="Command"/>s with the <see cref="CommandProcessor"/>. Adds each <see cref="Command"/>'s
        /// arguments, options, and validators to the command line interface.
        /// </summary>
        /// <param name="commands">The <see cref="Command"/>s to register with the <see cref="CommandProcessor"/>.</param>
        /// <returns>The <see cref="Task"/> for registering commands.</returns>
        public Task Register(IEnumerable<Command> commands)
        {
            foreach (var command in commands)
            {
                Register(command);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes the <see cref="CommandProcessor"/> using the arguments specified in the constructor and
        /// <see cref="Command"/>s registered. If valid arguments are provided, runs the corresponding <see cref="Command"/>.
        /// </summary>
        /// <returns>The <see cref="Task"/> for processing commands.</returns>
        public Task<int> Execute()
        {
            return _rootCommand.InvokeAsync(_arguments);
        }
    }
}

