using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Domain.ValueObjects
{
    public sealed class Block : ValueObject<Block>
    {
        public int Value { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Block(int value)
        {
            Value = value;
        }

        /// <summary>
        /// EqualsCore
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected override bool EqualsCore(Block other)
        {
            return this.Value == other.Value;
        }
    }
}
