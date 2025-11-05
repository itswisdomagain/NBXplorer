using System;
using System.Collections.Generic;
using NBitcoin;

namespace NBXplorer.DerivationStrategy
{
	public class P2SHDerivationStrategy : StandardDerivationStrategyBase
	{
		private Network network;
		bool addSuffix;
		internal P2SHDerivationStrategy(StandardDerivationStrategyBase inner, bool addSuffix, Network network) : base(inner.AdditionalOptions)
		{
			if (inner == null)
				throw new ArgumentNullException(nameof(inner));
			Inner = inner;
			this.addSuffix = addSuffix;
			this.network = network;
		}

		public StandardDerivationStrategyBase Inner
		{
			get; set;
		}

		protected internal override string StringValueCore
		{
			get
			{
				if (addSuffix)
					return Inner.StringValueCore + "-[p2sh]";
				return Inner.ToString();
			}
		}

		public override IEnumerable<ExtPubKey> GetExtPubKeys()
		{
			return Inner.GetExtPubKeys();
		}

		public override Derivation GetDerivation(KeyPath keyPath)
		{
			var derivation = Inner.GetDerivation(keyPath);
			return new KeyPathDerivation(
				 keyPath,
				 derivation.ScriptPubKey.Hash(network).ScriptPubKey,
				 derivation.Redeem ?? derivation.ScriptPubKey);
		}
	}
}
