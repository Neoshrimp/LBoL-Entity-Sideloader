using LBoL.ConfigData;
using LBoLEntitySideloader.Entities.ConfigBuilders.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LBoLEntitySideloader.Entities.ConfigBuilders.Piece
{
    public class PieceBuilder : ConfigBuilder<PieceConfig, PieceReadableConfig>
    {
        public PieceBuilder()
        {
            converterContainer = new PieceConverter();
        }

        public override PieceConfig BuildConfig(PieceReadableConfig readableConfig)
        {

            var pieceConfig = base.BuildConfig(readableConfig);

            var evStartList = new List<int[][]>();
            var evStartConvert = new EvStartConvert();

            var evDurationList = new List<int[][]>();
            var evDurationConvert = new EvDurationConvert();

            var eventNumberList = new List<float[][]>();
            var eventNumberConvert = new D2ArrayConverter<float>();

            var eventTypeList = new List<int[]>();
            var eventTypeConvert = new EventTypeConvert();

            foreach (var be in readableConfig.bulletEvents.bulletEvents)
            {
                evStartList.Add(evStartConvert.ConvertTo(be.evStart));
                evDurationList.Add(evDurationConvert.ConvertTo(be.evDuration));
                eventNumberList.Add(eventNumberConvert.ConvertTo(be.eventNumber));
                eventTypeList.Add(eventTypeConvert.ConvertTo(be.eventType));
            }

            pieceConfig.EvStart = evStartList.ToArray();
            pieceConfig.EvDuration = evDurationList.ToArray();
            pieceConfig.EvNumber = eventNumberList.ToArray();
            pieceConfig.EvType = eventTypeList.ToArray();


            return pieceConfig;

        }


        public override PieceReadableConfig Config2ReadableConfig(PieceConfig config)
        {
            var readableConfig = base.Config2ReadableConfig(config);


            var evStartConvert = new EvStartConvert();
            var evDurationConvert = new EvDurationConvert();
            var eventNumberConvert = new D2ArrayConverter<float>();
            var eventTypeConvert = new EventTypeConvert();

            readableConfig.bulletEvents = new BulletEvents();

            for (var i = 0; i < config.EvStart.Length; i++)
            {
                try
                {
                    readableConfig.bulletEvents.bulletEvents.Add(new DecodedBulletEvent()
                    {
                        evStart = evStartConvert.ReverseConvert(config.EvStart[i]),
                        evDuration = evDurationConvert.ReverseConvert(config.EvDuration[i]),
                        eventNumber = eventNumberConvert.ReverseConvert(config.EvNumber[i]),
                        eventType = eventTypeConvert.ReverseConvert(config.EvType[i])
                    });
                }
                catch (Exception ex)
                {

                    Log.log.LogError(ex);
                }

            }

            return readableConfig;
        }
    }
}
