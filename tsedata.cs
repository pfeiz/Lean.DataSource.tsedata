/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

using System;
using NodaTime;
using ProtoBuf;
using System.IO;
using QuantConnect.Data;
using System.Collections.Generic;

namespace QuantConnect.DataSource
{
    /// <summary>
    /// Example custom data type
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    public class MyStockDataset : BaseData
{
    public override DateTime EndTime { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public long Volume { get; set; }

    public override SubscriptionDataSource GetSource(SubscriptionDataConfig config, DateTime date, bool isLiveMode)
    {
        var path = Path.Combine(Globals.DataFolder, "alternative", "mystockdataset", $"{config.Symbol.Value.ToLowerInvariant()}.csv");
        return new SubscriptionDataSource(path, SubscriptionTransportMedium.LocalFile, FileFormat.Csv);
    }

    public override BaseData Reader(SubscriptionDataConfig config, string line, DateTime date, bool isLiveMode)
    {
        var parts = line.Split(',');

        var time = DateTime.ParseExact(parts[0], "yyyyMMdd", CultureInfo.InvariantCulture);

        return new MyStockDataset
        {
            Symbol = config.Symbol,
            Time = time,
            EndTime = time.AddDays(1),
            Open = Convert.ToDecimal(parts[1]),
            High = Convert.ToDecimal(parts[2]),
            Low = Convert.ToDecimal(parts[3]),
            Close = Convert.ToDecimal(parts[4]),
            Volume = Convert.ToInt64(parts[5]),
            Value = Convert.ToDecimal(parts[4])
        };
    }

    public override DateTimeZone DataTimeZone() => DateTimeZone.Utc;
    public override List<Resolution> SupportedResolutions() => new List<Resolution> { Resolution.Daily };
    public override Resolution DefaultResolution() => Resolution.Daily;
    public override bool IsSparseData() => true;
    public override bool RequiresMapping() => false;

    public override BaseData Clone()
    {
        return new MyStockDataset
        {
            Symbol = Symbol,
            Time = Time,
            EndTime = EndTime,
            Open = Open,
            High = High,
            Low = Low,
            Close = Close,
            Volume = Volume,
            Value = Value
        };
    }

    public override string ToString()
    {
        return $"{Symbol} - {Time:yyyy-MM-dd} O:{Open} H:{High} L:{Low} C:{Close} V:{Volume}";
    }
}
