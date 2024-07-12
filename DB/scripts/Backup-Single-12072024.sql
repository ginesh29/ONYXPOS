CREATE OR ALTER   Procedure [dbo].[PluFind]
@v_Cd		Char(24)
as
Begin --PluFind '4407220002757'

	 Declare @v_Itemcd		char(18)='0'
	 declare @v_Unit		char(6)
	 Declare @v_Dsc			char(100)
	 Declare @v_Price		Numeric(15,4)
	 Declare @v_Packqty		numeric(9,3)
	 Declare @v_PluDept		char(5)
	 Declare @v_PluVendCode	char(18)
	 Declare @TrnPrice		numeric(15,4)
	 Declare @Prom_Qty			int=0
	  Declare @Scallable		char(1)='N'
If (select count(1) from BarCode B where B.BarCode=@v_Cd)>0
Begin
	select @v_Itemcd=PluCode,@v_Unit=PluUom,@v_Dsc=BarDesc from BarCode where BarCode=trim(@v_Cd)
	select @v_Price=pluprice,@v_Packqty=PackQty from pluunit where PluCode=@v_Itemcd and PluUom=@v_Unit
	Select @v_PluDept=PluDept,@v_PluVendCode=PluVendCode from PluMaster where PluCode=@v_Itemcd
End
Else if (select count(1) from PluMaster  where PluBarCode=trim(@v_Cd))>0
Begin
	select @v_Itemcd=PluCode,@v_Unit=PluUom,@v_Dsc=PluName,@v_Price=PluPrice,@v_Packqty=1,@v_PluDept=PluDept,@v_PluVendCode=PluVendCode from PluMaster where PluBarCode=trim(@v_Cd)	
End

Select top 1
	@TrnPrice= isnull(PD.SellPr,0),
	@Prom_Qty=isnull(PD.Qty,0)
From
	dbo.PromotionHead PH with (nolock) INNER JOIN dbo.PromotionDetail PD with (nolock) ON PH.Cd = PD.PromCd
where  
	ItemCd= @v_Itemcd and Unit=@v_Unit and getdate() between StartDt and EndDt and Rtrim(Upper(PD.Typ))='ADD' and SchemeTyp='P_SCH09'

-----------------------Checking for the scale item Begin-----------------------------------------------------------------------------
Declare @WtPlu_Len int
Declare @WtQty_Len int
Declare @WtPr_Len int
select @WtPlu_Len =val from PosParameters where typ='WTPLU_LEN'
select @WtQty_Len =val from PosParameters where typ='WTQTY_LEN'
select @WtPr_Len =val  from PosParameters  where typ='WTPR_LEN'
if(@v_Itemcd='0')
Begin
Declare @v_Itemcd_N char(18)='0'
select @v_Itemcd_N=substring(@v_Cd,1,@WtPlu_Len)
If (select count(1) from BarCode B where B.BarCode=@v_Itemcd_N)>0
Begin
	select @v_Itemcd=PluCode,@v_Unit=PluUom,@v_Dsc=BarDesc from BarCode where BarCode=trim(@v_Itemcd_N)
	select @v_Price=pluprice,@v_Packqty=PackQty from pluunit where PluCode=@v_Itemcd_N and PluUom=@v_Unit
	Select @v_PluDept=PluDept,@v_PluVendCode=PluVendCode,@Scallable=Scalleable from PluMaster where PluCode=@v_Itemcd_N
End
Else if (select count(1) from PluMaster  where PluBarCode=trim(@v_Itemcd_N))>0
Begin
	select @v_Itemcd=PluCode,@v_Unit=PluUom,@v_Dsc=PluName,@v_Price=PluPrice,@v_Packqty=1,@v_PluDept=PluDept,@v_PluVendCode=PluVendCode,@Scallable=Scalleable from PluMaster where PluBarCode=trim(@v_Itemcd_N)	
End

End
-----------------------Checking for the scale item End-------------------------------------------------------------------------------



select 
	@v_Cd[Barcode]
,	@v_Itemcd[Itemcd]
,	@v_Unit[PluUom]
,	@v_Dsc[ItemName]
,	PluNameAr[ItemNameAr]
,	case isnull(@TrnPrice,0) when 0  then @v_Price  else @TrnPrice end [Price]
,	@v_Packqty[PackQty]
,	@v_PluDept[Dept]
,	@v_PluVendCode[Tax]
,	isnull(@TrnPrice,0)[PromPrice]
,	@v_Price[ActualPrice]
,	case isnull(@TrnPrice,0) when 0 then 0 else (@v_Price-isnull(@TrnPrice,0)) end[Discount]
,	@Prom_Qty[Prom_Qty]
,	@Scallable[Scalleable]
from PluMaster where PluCode=@v_Itemcd
End 
 Go 
