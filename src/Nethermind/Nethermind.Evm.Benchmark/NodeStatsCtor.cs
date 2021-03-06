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

using BenchmarkDotNet.Attributes;
using Nethermind.Core.Crypto;
using Nethermind.Core.Logging;
using Nethermind.Core.Model;
using Nethermind.Stats;
using Nethermind.Stats.Model;

namespace Nethermind.Evm.Benchmark
{
    [MemoryDiagnoser]
    [CoreJob(baseline: true)]
    public class NodeStatsCtorBenchmark
    {
        private ILogManager _logManager;
        private Node _node;
        private IStatsConfig _statsConfig;
        
        [GlobalSetup]
        public void Setup()
        {
            _node = new Node(new NodeId(new PublicKey("0x000102030405060708090a0b0c0d0e0f000102030405060708090a0b0c0d0e0f000102030405060708090a0b0c0d0e0f000102030405060708090a0b0c0d0e0f")));
            _statsConfig = new StatsConfig();
            _statsConfig.CaptureNodeStatsEventHistory = true;
            _statsConfig.CaptureNodeLatencyStatsEventHistory = true;
            _logManager = LimboLogs.Instance;
        }
        
        [Benchmark]
        public void Heavy()
        {
            NodeStats stats = new NodeStats(_node, _statsConfig, _logManager);
        }
        
        [Benchmark]
        public void Light()
        {
            NodeStatsLight stats = new NodeStatsLight(_node, _statsConfig, _logManager);
        }
        
        [Benchmark]
        public long HeavyRep()
        {
            NodeStats stats = new NodeStats(_node, _statsConfig, _logManager);
            return stats.CurrentNodeReputation;
        }
        
        [Benchmark]
        public long LightRep()
        {
            NodeStatsLight stats = new NodeStatsLight(_node, _statsConfig, _logManager);
            return stats.CurrentNodeReputation;
        }
    }
}