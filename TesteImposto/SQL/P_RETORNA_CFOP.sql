/*
4 – Exercício (Novo recurso)
*/

USE [Teste]
GO
IF OBJECT_ID('dbo.P_RETORNA_CFOP') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.P_RETORNA_CFOP
    IF OBJECT_ID('dbo.P_RETORNA_CFOP') IS NOT NULL
        PRINT '<<< FALHA APAGANDO A PROCEDURE dbo.P_RETORNA_CFOP >>>'
    ELSE
        PRINT '<<< PROCEDURE dbo.P_RETORNA_CFOP APAGADA >>>'
END
go
SET QUOTED_IDENTIFIER ON
GO
SET NOCOUNT ON 
GO 
CREATE PROCEDURE P_RETORNA_CFOP
AS
BEGIN
   select 
       [Cfop] as [Cfop], 
       sum(isnull([BaseIcms],0.00)) as [TotalBaseIcms],
	   sum(isnull([ValorIcms],0.00)) as [TotalValorIcms],
	   sum(isnull([BaseIpi],0.00)) as [TotalBaseIpi],
	   sum(isnull([ValorIpi],0.00)) as [TotalValorIpi]
   from 
       [dbo].[NotaFiscalItem]
   group by
	   [Cfop]
END

GO
GRANT EXECUTE ON dbo.P_RETORNA_CFOP TO [public]
go
IF OBJECT_ID('dbo.P_RETORNA_CFOP') IS NOT NULL
    PRINT '<<< PROCEDURE dbo.P_RETORNA_CFOP CRIADA >>>'
ELSE
    PRINT '<<< FALHA NA CRIACAO DA PROCEDURE dbo.P_RETORNA_CFOP >>>'
go