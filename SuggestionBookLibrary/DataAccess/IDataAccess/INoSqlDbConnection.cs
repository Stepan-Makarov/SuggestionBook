﻿using MongoDB.Driver;

namespace SuggestionBookLibrary.DataAccess.IDataAccess;
public interface INoSqlDbConnection
{
  IMongoCollection<CategoryModel> CategoryCollection { get; }
  string CategoryCollectionName { get; }
  MongoClient Client { get; }
  string DbName { get; }
  IMongoCollection<StatusModel> StatusCollection { get; }
  string StatusCollectionName { get; }
  IMongoCollection<SuggestionModel> SuggestionCollection { get; }
  string SuggestionCollectionName { get; }
  IMongoCollection<UserModel> UserCollection { get; }
  string UserCollectionName { get; }
}