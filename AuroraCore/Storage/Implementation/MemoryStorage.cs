// Aurora 
// Copyright (C) 2020  Frank Horrigan <https://github.com/saurer>

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using AuroraCore.Types;

namespace AuroraCore.Storage.Implementation {
    public partial class MemoryStorage : IStorageAdapter {
        private Dictionary<int, IEventData> events = new Dictionary<int, IEventData>();

        // Each key represents a collection of descendants(any level)
        private Dictionary<int, List<int>> subEventAncestors = new Dictionary<int, List<int>>();

        // Each key represents a direct descendant of a parent value
        private Dictionary<int, int> subEventChildren = new Dictionary<int, int>();

        // Each key represent a direct connection from container to provider
        private Dictionary<int, int> containerProviders = new Dictionary<int, int>();

        private IDataContext context;

        public MemoryStorage(ITypeManager typeManager) {
            context = new DataContext(this, typeManager);
        }
    }
}