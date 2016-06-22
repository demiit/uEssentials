/*
 *  This file is part of uEssentials project.
 *      https://uessentials.github.io/
 *
 *  Copyright (C) 2015-2016  Leonardosc
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License along
 *  with this program; if not, write to the Free Software Foundation, Inc.,
 *  51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using Essentials.Api.Command;
using Essentials.Api.Command.Source;

namespace Essentials.Api.Events
{
    public class CommandEvent
    {
        /// <summary>
        /// Command that was/will be executed.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Arguments
        /// </summary>
        public ICommandArgs Arguments { get; set; }

        /// <summary>
        /// Who is executing the <see cref="Command"/>.
        /// </summary>
        public ICommandSource Source { get; set; }

        public CommandEvent( ICommand command, ICommandArgs args, ICommandSource src )
        {
            Command = command;
            Arguments = args;
            Source = src;
        }
    }
}
