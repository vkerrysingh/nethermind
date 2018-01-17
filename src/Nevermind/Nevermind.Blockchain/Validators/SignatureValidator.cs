﻿/*
 * Copyright (c) 2018 Demerzel Solutions Limited
 * This file is part of the Nethermind library.
 *
 * The Nethermind library is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The Nethermind library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Numerics;
using Nevermind.Core;
using Nevermind.Core.Crypto;
using Nevermind.Core.Extensions;
using Nevermind.Core.Potocol;

namespace Nevermind.Blockchain.Validators
{
    public class SignatureValidator : ISignatureValidator
    {
        private readonly int _chainIdValue;

        private readonly IEthereumRelease _spec;

        public SignatureValidator(IEthereumRelease spec, int chainIdValue)
        {
            if (chainIdValue < 0)
            {
                throw new ArgumentException("Unexpected negative value", nameof(chainIdValue));
            }
            
            _spec = spec;
            _chainIdValue = chainIdValue;
        }

        public SignatureValidator(IEthereumRelease spec, ChainId chainId)
            : this(spec, (int)chainId)
        {
        }

        public bool Validate(Signature signature)
        {
            BigInteger sValue = signature.S.ToUnsignedBigInteger();
            BigInteger rValue = signature.R.ToUnsignedBigInteger();

            if (sValue.IsZero || sValue >= (_spec.IsEip2Enabled ? Secp256K1Curve.HalfN + 1 : Secp256K1Curve.N))
            {
                return false;
            }

            if (rValue.IsZero || rValue >= Secp256K1Curve.N - 1)
            {
                return false;
            }

            if (!_spec.IsEip155Enabled)
            {
                return signature.V == 27 || signature.V == 28;
            }

            return signature.V == 27 || signature.V == 28 || signature.V == 35 + 2 * _chainIdValue || signature.V == 36 + 2 * _chainIdValue;
        }
    }
}