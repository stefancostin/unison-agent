using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Data;
using Unison.Agent.Core.Models;
using Unison.Agent.Core.Models.Store;
using Unison.Common.Amqp.DTO;

namespace Unison.Agent.Core.Utilities
{
    public static class Mapper
    {
        #region Data.Fields
        public static AmqpField ToAmqpFieldModel(this Field field)
        {
            if (field == null)
                return null;

            return new AmqpField(name: field.Name, type: field.Type, value: field.Value);
        }

        public static StoreField ToStoreFieldModel(this Field field)
        {
            if (field == null)
                return null;

            return new StoreField(name: field.Name, type: field.Type, value: field.Value);
        }

        public static Field ToFieldModel(this AmqpField amqpField)
        {
            if (amqpField == null)
                return null;

            return new Field(name: amqpField.Name, type: amqpField.Type, value: amqpField.Value);
        }

        public static StoreField ToStoreFieldModel(this AmqpField amqpField)
        {
            if (amqpField == null)
                return null;

            return new StoreField(name: amqpField.Name, type: amqpField.Type, value: amqpField.Value);
        }

        public static Field ToFieldModel(this StoreField storeField)
        {
            if (storeField == null)
                return null;

            return new Field(name: storeField.Name, type: storeField.Type, value: storeField.Value);
        }

        public static AmqpField ToAmqpFieldModel(this StoreField storeField)
        {
            if (storeField == null)
                return null;

            return new AmqpField(name: storeField.Name, type: storeField.Type, value: storeField.Value);
        }

        public static Field ToFieldModel(this QueryParam schemaField)
        {
            if (schemaField == null)
                return null;

            return new Field(name: schemaField.Name, type: schemaField.Type, value: schemaField.Value);
        }

        public static QueryParam ToQueryParamModel(this AmqpField amqpField)
        {
            if (amqpField == null)
                return null;

            return new QueryParam(name: amqpField.Name, type: amqpField.Type, value: amqpField.Value);
        }
        #endregion

        #region Data.Records
        public static AmqpRecord ToAmqpRecordModel(this Record record)
        {
            if (record == null)
                return null;

            AmqpRecord amqpRecord = new AmqpRecord();

            if (record.Fields == null)
                return amqpRecord;

            amqpRecord.Fields = record.Fields.ToDictionary(f => f.Key, f => f.Value?.ToAmqpFieldModel());

            return amqpRecord;
        }

        public static Record ToRecordModel(this AmqpRecord amqpRecord)
        {
            if (amqpRecord == null)
                return null;

            Record record = new Record();

            if (amqpRecord.Fields == null)
                return record;

            record.Fields = amqpRecord.Fields.ToDictionary(f => f.Key, f => f.Value?.ToFieldModel());

            return record;
        }

        public static StoreRecord ToStoreRecordModel(this AmqpRecord amqpRecord)
        {
            if (amqpRecord == null)
                return null;

            StoreRecord storeRecord = new StoreRecord();

            if (amqpRecord.Fields == null)
                return storeRecord;

            var dictionary = amqpRecord.Fields.ToDictionary(f => f.Key, f => f.Value?.ToStoreFieldModel());
            storeRecord.Fields = new ConcurrentDictionary<string, StoreField>(dictionary);

            return storeRecord;
        }

        public static Record ToRecordModel(this StoreRecord storeRecord)
        {
            if (storeRecord == null)
                return null;

            Record record = new Record();

            if (storeRecord.Fields == null)
                return record;

            record.Fields = storeRecord.Fields.ToDictionary(f => f.Key, f => f.Value?.ToFieldModel());

            return record;
        }

        public static QueryRecord ToQueryRecordModel(this AmqpRecord amqpRecord)
        {
            if (amqpRecord == null)
                return null;

            QueryRecord record = new QueryRecord();

            if (amqpRecord.Fields == null)
                return record;

            record.Fields = amqpRecord.Fields.Select(f => f.Value?.ToQueryParamModel()).ToList();

            return record;
        }
        #endregion

        #region Data.DataSets
        public static AmqpDataSet ToAmqpDataSetModel(this DataSet dataSet)
        {
            if (dataSet == null)
                return null;

            AmqpDataSet amqpDataSet = new AmqpDataSet(entity: dataSet.Entity, primaryKey: dataSet.PrimaryKey);

            if (dataSet.Records == null)
                return amqpDataSet;

            amqpDataSet.Records = dataSet.Records.ToDictionary(r => r.Key, r => r.Value?.ToAmqpRecordModel());

            return amqpDataSet;
        }

        public static DataSet ToDataSetModel(this AmqpDataSet amqpDataSet)
        {
            if (amqpDataSet == null)
                return null;

            DataSet dataSet = new DataSet(entity: amqpDataSet.Entity, primaryKey: amqpDataSet.PrimaryKey);

            if (amqpDataSet.Records == null)
                return dataSet;

            dataSet.Records = amqpDataSet.Records.ToDictionary(r => r.Key, r => r.Value?.ToRecordModel());

            return dataSet;
        }

        public static StoreDataSet ToStoreEntityModel(this AmqpDataSet amqpDataSet)
        {
            if (amqpDataSet == null)
                return null;

            StoreDataSet storeDataSet = new StoreDataSet(entity: amqpDataSet.Entity, primaryKey: amqpDataSet.PrimaryKey);

            if (amqpDataSet.Records == null)
                return storeDataSet;

            var dictionary = amqpDataSet.Records.ToDictionary(r => r.Key, r => r.Value?.ToStoreRecordModel());
            storeDataSet.Records = new ConcurrentDictionary<string, StoreRecord>(dictionary);

            return storeDataSet;
        }

        public static DataSet ToDataSetModel(this StoreDataSet storeDataSet)
        {
            if (storeDataSet == null)
                return null;

            DataSet dataSet = new DataSet(entity: storeDataSet.Entity, primaryKey: storeDataSet.PrimaryKey);

            if (storeDataSet.Records == null)
                return dataSet;

            dataSet.Records = storeDataSet.Records.ToDictionary(r => r.Key, r => r.Value?.ToRecordModel());

            return dataSet;
        }

        public static QuerySchema ToQuerySchema(this AmqpDataSet amqpDataSet, QueryOperation operation)
        {
            if (amqpDataSet == null)
                return null;

            QuerySchema schema = new QuerySchema()
            {
                Entity = amqpDataSet.Entity,
                PrimaryKey = amqpDataSet.PrimaryKey,
                Operation = operation
            };

            if (amqpDataSet.Records == null || !amqpDataSet.Records.Any())
                return schema;

            schema.Fields = amqpDataSet.Records.First().Value.Fields.Select(f => f.Value.Name).ToList();
            schema.Records = amqpDataSet.Records.Select(r => r.Value?.ToQueryRecordModel()).ToList();

            return schema;
        }
        #endregion

        #region Amqp.Messages
        public static QuerySchema ToQuerySchema(this AmqpSyncRequest message)
        {
            return new QuerySchema()
            {
                Entity = message.Entity,
                Fields = message.Fields,
                PrimaryKey = message.PrimaryKey
            };
        }

        public static AmqpSyncState ToAmqpSyncState(this QuerySchema schema)
        {
            AmqpSyncState syncState = new AmqpSyncState();
            syncState.Added = new AmqpDataSet(schema.Entity, schema.PrimaryKey);
            syncState.Updated = new AmqpDataSet(schema.Entity, schema.PrimaryKey);
            syncState.Deleted = new AmqpDataSet(schema.Entity, schema.PrimaryKey);
            return syncState;
        }
        #endregion
    }
}
