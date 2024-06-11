CREATE OR ALTER Proc [dbo].[GetPromo_Scheme99]
	@Bill		Varchar(10)
,   @Dt			Date
As	-- Drop Proc [dbo].[GetPromo_Scheme99] '25','2023-03-04'
/*
	DECLARE	@Bill		Varchar(10)='166'
		,   @Dt			Date='2023/12/29'
*/
Begin
	Declare @count		int=1
		,	@max		int
		,	@maxtr		int
		,	@itemcd		char(18)
		,	@unit		char(6)
		,	@trnQty		numeric(15,3)
		,	@pQty		numeric(15,3)
		,	@prate		numeric(15,4)
		,	@TrnSlNo	numeric(5,0)
		,	@vat		numeric(18,2)

		,	@PromoNo			varchar(10)
		,	@mTrnSlNo			numeric(5,0)
		,	@mTrnQty			numeric(12,3)
		,	@mSellPr			numeric(12,3)
		,	@mTrnPrice			numeric(12,3)
		,	@mTrnNetVal			numeric(12,3)
		,	@mVatPerc			numeric(5,2)
		,	@mTrnAmt			numeric(10,3)
		,	@gLoyaltySavings	numeric(10,2)
		,	@mTrnNetVal_New		numeric(12,3)
		,	@mTrnAmt_New		numeric(12,3)
		,	@gDecimals			numeric(1)		--	Select P_Decimals from posctrl

	--Select Cd, des Into #Temp_Promo from PromotionHead Where SchemeTyp='P_SCH10' and 
	--	(Getdate() between StartDt and EndDt+1 and Convert(Varchar(10),EndDt,108)='00:00:00' or
	--	 Getdate() between StartDt and EndDt and Convert(Varchar(10),EndDt,108)!='00:00:00')

    --Select Top 1 @PromoNo=Cd From PromotionHead Where SchemeTyp = 'P_SCH99' and 
	if (Select Count(1) From PosTrans) > 0
		Select Top 1 @Dt=TrnDt From PosTrans Order By TrnNo
	else if (Select Count(1) From Shift) > 0		--Select * From Shift
		Select Top 1 @Dt=StDate From Shift Order By ShiftNo

    Select Cd[PromoNo] Into #Temp_PromHead From PromotionHead Where SchemeTyp = 'P_SCH99' and 
			Convert(Varchar(10),StartDt,112) <= Convert(Varchar(10),@Dt,112) and 
			Convert(Varchar(10),EndDt,112) >= Convert(Varchar(10),@Dt,112)  

    Select
		TrnNo
	,	TrnSlNo
	,	TrnPlu
	,	TrnUnit
	,	ItemCd
	,	Unit
	,	TrnQty
	,	TrnPrice
	,	TrnNetVal
	,	TrnAmt
	,	SellPr
	,	(Select Cast(PluVendCode as Decimal) From PluMaster Where PluCode=TrnPlu)[TrnLDiscPercent]
	Into
		#Temp_Promo
	From
		PosTemp Join PromotionDetail on Rtrim(TrnPlu)=Rtrim(ItemCd) 
			and Rtrim(TrnUnit)=Rtrim(Unit)
    Where
		--PromCd = @PromoNo And TrnPrice > SellPr And SellPr >0
		PromCd in (Select PromoNo From #Temp_PromHead) And TrnPrice > SellPr And SellPr >0

	Alter Table #Temp_Promo Add Srl int identity(1,1)

	Set @gLoyaltySavings = 0
	Select Top 1 @gDecimals=P_Decimals from PosCtrl
	--- select * from postemp
	select @max=Max(Srl) From #Temp_Promo
	While(@count<=@max)
	  Begin
		Select 
			@mTrnSlNo = TrnSlNo
		,	@mTrnQty = TrnQty
        ,	@mSellPr = SellPr
        ,	@mTrnPrice = TrnPrice
        ,	@mTrnNetVal = TrnNetVal
        ,	@mVatPerc = TrnLDiscPercent
        ,	@mTrnAmt = TrnAmt
		From
			#Temp_Promo
		where
			Srl=@count

		set @count=@count+1
        If @mTrnPrice > 0
		  Begin
            Update PosTemp Set TrnTDiscType='L', TrnPrice=@mSellPr Where TrnSlNo=@mTrnSlNo
            Set @mTrnNetVal_New = Round(@mTrnQty * @mSellPr * 100 / (100 + @mVatPerc), @gDecimals)    
            Set @mTrnAmt_New = Round(@mTrnQty * @mSellPr * @mVatPerc / (100 + @mVatPerc), @gDecimals)
            Update PosTemp Set TrnNetVal=@mTrnNetVal_New, TrnAmt = @mTrnAmt_New Where TrnSlNo=@mTrnSlNo
            Set @gLoyaltySavings = @gLoyaltySavings + @mTrnNetVal -@mTrnNetVal_New
		  End
	  End
	Select @gLoyaltySavings
	Drop Table #Temp_Promo
	Drop Table #Temp_PromHead	
	
End
 
 Go 


 CREATE OR ALTER Proc [dbo].[Ly_GetItemPromotion]  -- '100009635','pcs'
	@v_Cd		Varchar(50)
,	@v_Unit		Varchar(50) = null
,	@v_Terminal	varchar(10) = null 
As	-- Drop Proc Ly_GetItemPromotion '100009635','pcs'
Begin
	Declare @ItemCd		varchar(20)
	declare @Qty		int
	declare @TrnQty		int
	declare @pStatus	int
	declare @TrnPrice	numeric(18,3)
	Declare @barCd		varchar(50)
		,   @SchemeTyp      char(10)
		,	@Dt				datetime
		,	@time			varchar(10)
	Set @v_Unit=Isnull(@v_Unit,'')
	if (Select Count(1) From PosTemp With (NoLock)) > 0 
		Select Top 1 @Dt=TrnDt, @time=TrnTime From PosTemp With (NoLock)
	else
		Select Top 1 @Dt=StDate, @time=StTime From Shift With (NoLock) Order by StDate, StTime
		-- Select * From Shift With (NoLock) Order by StDate, StTime

	Set @Dt	= Cast((Convert(Varchar(10),@Dt,111) +' '+@time) as datetime)

	Select * Into #Temp_Promo from PromotionHead 
		Where (@Dt between StartDt and EndDt+1 and Convert(Varchar(10),EndDt,108)='00:00:00' or
				Getdate() between StartDt and EndDt and (Convert(Varchar(10),StartDt,108)!='00:00:00' 
												or Convert(Varchar(10),EndDt,108)!='00:00:00'))

	Set @barCd=@v_Cd
	Select @v_Cd=isnull(PluCode,'0') from plumaster where (PluBarCode=@v_Cd or PluCode=@v_Cd) and pluUom=@v_Unit
	--print @v_Cd

	Select
		@Qty= isnull(PD.Qty,0)
	,	@SchemeTyp=SchemeTyp
	,	@TrnPrice=PD.SellPr
	From
		#Temp_Promo PH INNER JOIN dbo.PromotionDetail PD ON PH.Cd = PD.PromCd
	where
		ItemCd= @v_Cd and Unit=@v_Unit and Rtrim(Upper(PD.Typ))='ADD'
	and SchemeTyp='P_SCH09' -- and getdate() between StartDt and EndDt+1

	select @TrnQty=isnull(sum(TrnQty),0) From [dbo].[PosTemp] where (TrnPlu=@v_Cd or TrnBarcode=@barCd) and TrnUnit=@v_Unit

	set @pStatus=0
	if (@Qty=0 or (@TrnQty<=@Qty)) and (@SchemeTyp='P_SCH09')
	  Begin
		Select
			PH.Cd
		,	PH.Loc
		,	PH.Des
		,	PH.DiscPerc
		,	PH.StartDt
		,	PH.EndDt
		,	PD.SrNo
		,	PD.Grp
		,	PD.ItemCd
		,	PD.Qty
		,	PD.Unit
		,	PD.SellPr
		,	PD.DiscPerc
		,	@pStatus[pStatus]
		,	PD.Typ
		,	@TrnQty[TrnQty]
		From
			#Temp_Promo PH INNER JOIN dbo.PromotionDetail PD ON PH.Cd = PD.PromCd 
		Where
			ItemCd= @v_Cd and Unit=@v_Unit and SchemeTyp='P_SCH09' --and getdate() between StartDt and EndDt+1
	  End
	else
	  Begin
		if (@SchemeTyp='P_SCH09' or (@TrnQty>@Qty) )
		  Begin
			set @pStatus=1
			If (select count(1) from [dbo].[PluMaster] where (PluBarCode=@v_Cd or PluCode=@v_Cd) and pluUom=@v_Unit) > 0 
				select @TrnPrice=PluPrice from [dbo].[PluMaster] where (PluBarCode=@v_Cd or PluCode=@v_Cd) and pluUom=@v_Unit
			If (select count(1) from [dbo].[PluUnit] where PluCode=@v_Cd and PluUom=@v_Unit) > 0 
				select @TrnPrice=PluPrice from [dbo].[PluUnit] where PluCode=@v_Cd and pluUom=@v_Unit
			If (select count(1) from [dbo].[BarCode] where BarCode=@v_Cd and PluUom=@v_Unit) > 0 
				select @TrnPrice=PluPrice from [dbo].[PluUnit] PU INNER JOIN [dbo].[BarCode] BC ON BC.PluCode=PU.PluCode and BC.PluUom=PU.PluUom
					where BC.PluCode=@v_Cd and BC.PluUom=@v_Unit
		  End
		Select
			PH.Cd
		,	PH.Loc
		,	PH.Des
		,	PH.DiscPerc
		,	PH.StartDt
		,	PH.EndDt
		,	PD.SrNo
		,	PD.Grp
		,	PD.ItemCd
		,	PD.Qty
		,	PD.Unit
		,	@TrnPrice[SellPr]
		,	PD.DiscPerc
		,	@pStatus[pStatus]
		,	PD.Typ
		,	@TrnQty[TrnQty]
		From
			#Temp_Promo PH INNER JOIN dbo.PromotionDetail PD ON PH.Cd = PD.PromCd
		Where 
			SchemeTyp='P_SCH09'
		and	ItemCd= @v_Cd and Isnull(Unit,'')=@v_Unit -- and getdate() between StartDt and EndDt+1
	  End
	Drop Table #Temp_Promo

End
 
 Go 
CREATE OR ALTER  Proc [dbo].[GetLoyaltyInitialPromotionDiscount] 
	@BillNo		Numeric(10)
,	@Discount   Numeric(18,5)

As
--		-- Drop Proc GetLoyaltyInitialPromotionDiscount 76,10	-- sp_help Postrans
Begin


Declare		@count		int=1
		,	@max		int
		,	@maxtr		int
		,	@itemcd		char(18)
		,	@unit		char(6)
		,	@trnQty		numeric(15,3)
		,	@pQty		numeric(15,3)
		,	@prate		numeric(15,4)
		,	@TrnSlNo	numeric(5,0)
		,	@vat		numeric(18,2)

	
		,	@mTrnSlNo			numeric(5,0)
		,	@mTrnQty			numeric(12,3)
		,	@mSellPr			numeric(12,3)
		,	@mTrnPrice			numeric(12,3)
		,	@mTrnNetVal			numeric(12,3)
		,	@mVarPerc			numeric(5,2)
		,	@mTrnAmt			numeric(10,3)
		,	@gLoyaltySavings	numeric(10,2)
		,	@mTrnNetVal_New		numeric(12,3)
		,	@mTrnAmt_New		numeric(12,3)
		,	@gDecimals			numeric(1)	
		,	@mTaxPer			numeric(10,3)
		,	@mTrnNetVal_Actual		numeric(12,3)
		,	@mTrnAmt_Actual 	numeric(12,3)
		

Select Top 1 @gDecimals=P_Decimals from PosCtrl
Set @gLoyaltySavings = 0
 Select
		*,	(Select Cast(PluVendCode as Decimal) From PluMaster Where PluCode=TrnPlu)[TaxPer]
	Into
		#Temp_Promo
	From
		PosTemp  where TrnNo=@BillNo 
--Inserting to PostransDiscount to get complete bill which is having discount--
--insert into PostransDiscount 
--select 
--TrnNo
--,TrnSlNo
--,TrnDt
--,TrnDept
--,TrnPlu
--,TrnQty
--,TrnPrice
--,TrnUnit
--,TrnPackQty
--,TrnPrLvl
--,TrnLDisc
--,TrnTDisc
--,TrnLDiscPercent
--,TrnTDiscType
--,TrnMode
--,TrnType
--,TrnDeptPlu
--,TrnNetVal
--,0
--,TrnUser
--,TrnTime
--,TrnErrPlu
--,TrnLoc
--,TrnPosId
--,TrnShift
--,TrnAmt
--,0
--,TrnParty
--,TrnSalesman
--,TrnDesc
--,TrnFlag
--,TrnName
--,TrnBarcode
--from PosTemp  where TrnNo=@BillNo  
--Insertion completed to get complete bill which is having discount--
	Alter Table #Temp_Promo Add Srl int identity(1,1)--select * from postemp
	select @max=Max(Srl) From #Temp_Promo
	While(@count<=@max)
	  Begin
		Select 
			@mTrnSlNo = TrnSlNo
		,	@mTrnQty = TrnQty
        ,	@mTrnPrice = TrnPrice
        ,	@mTrnNetVal = TrnNetVal
        ,	@mVarPerc = @Discount
        ,	@mTrnAmt = TrnAmt
		,	@mTaxPer=TaxPer
			
		From
			#Temp_Promo
		where
			Srl=@count

		set @count=@count+1
        If @mTrnPrice > 0
		  Begin
			--update PostransDiscount set  TrnTDiscType='L' Where TrnSlNo=@mTrnSlNo
			set @mTrnNetVal_Actual=round((((@mTrnPrice*@mTrnQty)*100/(100+@mTaxPer))), 2)
			set @mTrnAmt_Actual=round((@mTrnPrice*@mTrnQty)-@mTrnNetVal_Actual, 2)

            Update PosTemp Set TrnTDiscType='L' Where TrnSlNo=@mTrnSlNo
			  set @mTrnNetVal_New = Round((@mTrnNetVal_Actual)-((@mTrnNetVal_Actual*@mVarPerc/100)), 2)  
            --Set @mTrnNetVal_New = Round(@mTrnNetVal-(@mTrnNetVal*(@mVarPerc/100)), @gDecimals)  
			 If @mTrnAmt <> 0
            Set @mTrnAmt_New = Round(@mTrnAmt_Actual-(@mTrnAmt_Actual*(@mVarPerc/100)) , 2)
			else
			set @mTrnAmt_New=@mTrnAmt
			--update PostransDiscount set  TrnNetVal_N=@mTrnNetVal_New,TrnAmt_N = @mTrnAmt_New Where TrnSlNo=@mTrnSlNo
            Update PosTemp Set TrnNetVal=@mTrnNetVal_New, TrnAmt = @mTrnAmt_New Where TrnSlNo=@mTrnSlNo
            Set @gLoyaltySavings = @gLoyaltySavings + @mTrnNetVal_Actual+@mTrnAmt_Actual -(@mTrnNetVal_New-@mTrnAmt_New)
		  End
	  End
	Select sum(trnnetval+trnamt)[Savings] from postemp where trnno=@BillNo
	Drop Table #Temp_Promo
End 
 Go 
