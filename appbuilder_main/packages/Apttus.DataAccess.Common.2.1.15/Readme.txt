Release Notes for Apttus.DataAccess.Common.1.4.2
------------------------------------------------
* Making all projects to support x64
* PLAT-1867 Bug fixes 
* DbContextParams added to DbContext

Release Notes for Apttus.DataAccess.Common.1.3.2
------------------------------------------------
* Breaking changes for certain products due to refactoring of some method signatures.
* PLAT-1814 Support where condition between two tables


Release Notes for Apttus.DataAccess.Common.1.3.0
------------------------------------------------
PLAT-1810 GetRecordsAsync on IObjectinstance for empty resultset throws error while it should have returned empty list	
PLAT-1811 QueryBuilder should not be cached in SqlDataRepository


Release Notes for Apttus.DataAccess.Common.1.2.8
------------------------------------------------
PLAT-1765 System fields ModifiedBy and Createdby as FK again.

Release Notes for Apttus.DataAccess.Common.1.2.7
------------------------------------------------

*             Additional fixes for Dynamics CRM Query Builder
*   PLAT-1762 Support AddCondition(Condition condition)
*             PLAT-1764 Support for TopRecords and query.AddCriteria(Expression e)
                                                  New method added to Query "public void AddSortOrder(string field, SortOrder sortOrder) {"

Release Notes for Apttus.DataAccess.Common.1.2.3
------------------------------------------------

*             With this version we removed Transaction handling capability and let clients use TransactionScopes instead. Examples to be provided
*             GetAllColumnNames() introduced to get all column names from an entity (<Client's sub>entity + BaseEntity fields)

Release Notes for Apttus.DataAccess.Common.1.2.0
------------------------------------------------

*             This version has breaking changes.
*             This version has Composite types for CreatedBy,ModifiedBy and Owner.
*             This version has changes to Query.AddCriteria renamed to Query.SetCriteria.
*             This version has some major refactoring to renaming FilterExpression -> Expression,
                ConditionExpression -> Condition.
*             Also the DbContext has been implemented. This will be the lightweight context object derived 
                from ApttusRequestContext.
