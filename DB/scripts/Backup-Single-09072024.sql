CREATE OR ALTER   Procedure PluFind
@v_Cd		Char(24)
as
Begin --PluFind '6085011010228'

	 Declare @v_Itemcd		char(18)
	 declare @v_Unit		char(6)
	 Declare @v_Dsc			char(100)
	 Declare @v_Price		Numeric(15,4)
	 Declare @v_Packqty		numeric(9,3)
	 Declare @v_PluDept		char(5)
	 Declare @v_PluVendCode	char(18)
	 Declare @TrnPrice		numeric(15,4)	
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
	@TrnPrice= isnull(PD.SellPr,0)
From
	dbo.PromotionHead PH with (nolock) INNER JOIN dbo.PromotionDetail PD with (nolock) ON PH.Cd = PD.PromCd
where  
	ItemCd= @v_Itemcd and Unit=@v_Unit and getdate() between StartDt and EndDt

select @v_Cd[Barcode],@v_Itemcd[Itemcd],@v_Unit[PluUom],@v_Dsc[ItemName],PluNameAr[ItemNameAr],
case isnull(@TrnPrice,0) when 0  then @v_Price  else @TrnPrice end [Price],@v_Packqty[PackQty],@v_PluDept[Dept],@v_PluVendCode[Tax]
,isnull(@TrnPrice,0)[PromPrice],@v_Price[ActualPrice],
case isnull(@TrnPrice,0) when 0 then 0 else 
(@v_Price-isnull(@TrnPrice,0)) end[Discount]
from PluMaster where PluCode=@v_Itemcd
End 
 Go 
