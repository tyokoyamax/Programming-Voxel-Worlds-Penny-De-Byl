using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Domain.ValueObjects
{
    public sealed class BlockType : ValueObject<BlockType>
    {
        public static BlockType GRASS { get; } = new BlockType(0);

        public static BlockType DIRT { get; } = new BlockType(1);

        public static BlockType STONE { get; } = new BlockType(2);

        public static BlockType SAND { get; } = new BlockType(3);

        public static BlockType DIAMOND { get; } = new BlockType(4);

        public static BlockType AIR { get; } = new BlockType(5);


        private int Value { get; }

        private BlockType(int aValue)
        {
            Value = aValue;
        }

        protected override bool EqualsCore(BlockType other)
        {
            return Value == other.Value;
        }
    }
}
