﻿
CREATE TABLE [dbo].[Invoices] (
    [Id] [int] NOT NULL IDENTITY,
    [FlightId] [int] NOT NULL,
    [ParentId] [int],
    [ChildId] [int],
    [InvoiceNumber] [nvarchar](max),
    [CustomerId] [int] NOT NULL,
    [CustomerName] [nvarchar](max),
    [TaxCode] [nvarchar](max),
    [Address] [nvarchar](max),
    [Volume] [decimal](18, 2) NOT NULL,
    [Weight] [decimal](18, 2) NOT NULL,
    [Gallon] [decimal](18, 2) NOT NULL,
    [Temperature] [decimal](18, 2) NOT NULL,
    [Density] [decimal](18, 2) NOT NULL,
    [ProductName] [nvarchar](max),
    [Price] [decimal](18, 2) NOT NULL,
    [TaxRate] [decimal](18, 2) NOT NULL,
    [SubTotal] [decimal](18, 2) NOT NULL,
    [Tax] [decimal](18, 2) NOT NULL,
    [DateCreated] [datetime] NOT NULL,
    [DateUpdated] [datetime] NOT NULL,
    [UserCreatedId] [int],
    [UserUpdatedId] [int],
    [IsDeleted] [bit] NOT NULL,
    [DateDeleted] [datetime],
    [UserDeletedId] [int],
    [ChildInvoice_Id] [int],
    CONSTRAINT [PK_dbo.Invoices] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_ParentId] ON [dbo].[Invoices]([ParentId])
CREATE INDEX [IX_ChildInvoice_Id] ON [dbo].[Invoices]([ChildInvoice_Id])
ALTER TABLE [dbo].[Invoices] ADD CONSTRAINT [FK_dbo.Invoices_dbo.Invoices_ChildInvoice_Id] FOREIGN KEY ([ChildInvoice_Id]) REFERENCES [dbo].[Invoices] ([Id])
ALTER TABLE [dbo].[Invoices] ADD CONSTRAINT [FK_dbo.Invoices_dbo.Invoices_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Invoices] ([Id])
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'202011300153579_invoice', N'Megatech.Nafsc.Data.Migrations.Configuration',  0x1F8B0800000000000400ED7DDD8EDC3A92E6FD02FB0E89BCDA6D9C71BACEE906BA8DF20CAAABCA3EB55D3FB99565F7F45541CE64A585A3947224A5BB8CC53CD95CEC23ED2BAC2852127F8222295152A60F61C0A8E44F2848063F06C988E0FFFBAFFF7BFE6FAFBB68F60DA55998C4EFE7676FDECE67285E279B30DEBE9F1FF2977FF9F3FCDFFEF5BFFFB7F3EBCDEE75F6B92AF70B2E57D48CB3F7F3AF79BE7FB75864EBAF6817646F76E13A4DB2E4257FB34E768B60932C7E7EFBF62F8BB3B3052A48CC0B5AB3D9F9E321CEC31D2A7F143F2F93788DF6F92188EE920D8A329A5EE4AC4AAAB3FB6087B27DB046EFE71FEE566FAE823C98CF2EA23028BEBF42D1CB7C16C471920779C1DDBB4F195AE569126F57FB2221889EBEEF5151EE25883244B97ED714376DC0DB9F7103164DC58AD4FA90E5C9CE92E0D92FB4471662F54EFD3AAF7BACE8B3EBA26FF3EFB8D565BFBD9F5F84E93A0D5EF2F94CFCD8BBCB28C5059B6E7D5315FE697687B6418ED65FDFDC072FD9BACCFDA916814252F0BF9F669787283FA4E87D8C0E791A443FCD96872F51B8FE1BFAFE94FC86E2F7F1218A58F60A068B3C2EA1485AA6C91EA5F9F747F44299BED9CC670BBEDE42AC585763EA90E6DCC4F92F3FCF67F7C5C7832F11AA479F69FA2A4F52F411C5282D5AB95906798ED218D34065FF495F17BE85FFAFBE56885B315FE6B3BBE0F516C5DBFCEBFB79F1E77CF6217C459B2A8572F0290E8BE95554CAD303D27DE4B228E9E0235237B47FB51201FCDDF1BF7E59CA284AA5C16CAF568827BA4C111ECBAA1E4E7A0A77F62CE08A9FF61B27B40A344A295F962DC235291796356FB22B142186FBBF26498482B8534708A49A8ED0734FABEAB8BF0FBE85DB72522A44613E7B44515920FB1AEE09E463A08AC2183D57D29A15C29826BBC7242222CC673E3F05E9161510F894A84AAC9243BA96DBA5E4EE43146EBFE2CF02CC91BCE706791BD6842C8931311F62EB7CD180BC0EFA712B4D911F97F5C0DF2ED8647C3032D3AABA4F8F02F147B0583D05AFA334E662B34951960DFE9D9BF85B12AED1287D47BF355617D2CF8DD5937E6DFEF1D66666D9EDBF38576B9C7A71AE5649B78B33591BA1B5B9FCBC7A692EB37BADCC8496D1C24C8AFA75F918F74A57681FA4B8BFF149C5E610A10D998076139252A90ADB55BE48D3F05B10F56280D2E852954867979A8F49D1E25196573A632DA1BA429F8ED5C65186DCEED5DB3F769DE5E1AE18DC8B5D7288F37AB8D1BA488DE6B3655AFC450F12FF3C9FADD6019E46F62A71813907D44F9E0512BF16489D75A3D1E5EBCB20FDAD1881DBC4567468C5C107F273107D43CBB01EC272596C126D31B0C00E948EB3C72896BFA27C2158E37C6FF5357CC9C7F9CA2A2F96802ED256D6BE8EBBE17E98EE93341F67ECD2C3FAB77146ADD0280E1927DC6495AA32EC0F44712F594EE6A742B1890686CA651AAED160D45B44D29292523C2DE9DC27F908A7EE644B789BACE98D0C2B47B7371F7F7D7ABE7CBCBE78BABE7ABE7DB8BC78BA79B8B76E08D59AD855BB1153926C49F1218C2E93DD3E88BF7314D9E40E3B4CBF6DFF31B7ED7D4FADC51DBBEA54DB822FB219EFB35F5733C56DE72D78C2B0AFE189148178C2392D3C95D9B63C35AA25C85693FD5C9F8034ACC9B9D2110750C4F65E84E8CD3739DAC1C72F4DFE737508D2B028654A1CCA25FADE90901134BB21C165FD49CC319EC438D22C6D55437F84EED7E28E6B31A6014324CEA996080EC1B90C091AF9DC5EB088491961222EE801512F2CA36C7C3F146D980607AF7741180D7F6F995DC798A9DE33FF36C8F2DB641BC6B6F3BEDB51C05598EDA3E0FB2832B044E92ECC3271E3FA6975FDF8BCBC7EBCBB59ADBA6C58FD02F4E32D406D1B1C762D51AD40D2E686CBECB8DDEAB522B631045F221BAF88ECCECB605D6C8AFBD5B11BA4BA35A4FAF9ED049B8EB3B7665FF5D0EBA1173CC4010058CE95500F28E2D27EC6FA90A9853FF11CCA94BFE670BE9DC1A61CC86195DDC6625DA6D7F2F111250D2F06EB0753DE2F201A653E89B7617E60E0BF18B9C81ECF6E0B6E1C90F1707D9A706D3C959B1368A399DC14F713D9C042A0AF1EE8EED27AE0BBFB27B4DBE38E2A467AD81BFCFBC3EE0B3ECD1BE81BD7F166E02FE00B6C2728B22C347407643A1ADB103B294B047D286524B1768743DFC235EA6C5344AA776C27607EF378FDE1D3F5EDF3CDD3F5DDF3EAE9E2E9D3CABAD3EF82F81044634C9A2B1467255A19D1FF6347DB8FBE3837ACE9CFFFBEBC4F86B7080B5E1F837CB8465CECF769F22D88B006606BC44BAB02B25C0AF1C572F9F8F0F9BAAB3057E427B7672A1BD3DB9AA95171248B2676E63FFD63796D4DFB73121D76C3C908217FF6A7C13EF077446C1C0622FF3188A2664CDD2FEEAF8506BA1E8E7DBF593ACDCD52DBD90CD17374B63F5529D0F687644AA73272896EE75A2EEC925A78132C974C79AB543D1D774D3990BF2ABB8DC3BA8C2D8FE5164DC7202D047257E6B5B1460AF43A6BA3DF37D89B9725FDB6DC6433606BFD8E996194DC1F2C464A4176E0E381CB439A16E333F057BAD933147D37BC570C15BBE5F01613E5A9403AC297C8A458A1340C863796A14700237DCCEB713F9C1E572150CB552559ACC582CDC20FE54B8B3F58C8563121D2DEC2645540648EA42B98A299BDB491EAC306EA0829EAF511CDF9620969FC99F348BE84DD8EEA3C389E26381A4FF1CBAF41BC45B7C9D66896D7A5FD44D7DDB3E0521A1B2B47A10DCA4F513BDD11344EF27394A61523FA39880E68782795876833D297EED13F47FA5289DDE5847583B7DD5093A93980CC18E31C8D1B668472B4ACC7B8512E0303AC3B5B0AD5E5D730B25EBE6950BC71942F652CD88E316547C1DB1F2D0EE2C01770A77D3F767CF6085D2C0992CD613D4E109861AD16863628581DBE94515D866C80BF6AF5BB50F323BA7209AFB432E0EC8BE63DF3059B3330285F3A0B030B75885C815514035E859232B35C0125B77CA97EBE5F0421297899787F3115BC06DC51C9335AB59C980B5FB18B468F1857309D295744F751A1B8BBBFD23533EE1033F0628BE2F5F7BE83D7D11BDB2F843FDC4248E409F60663B0F8B92AC77883C9D9B2371850C676FD6B7DCC82FB405352C16555A09DCFBA94F54A4DA8E819AD0B2AF8A43FDAD9AC0AF55AA0AB613509E25416F58BF22856359A535C7F1CD321068A7F96C2DBACB4D4FCF1F500DB6D9BCD8ECDAF0BC7686DE908ECFC8199C707161FB093DBAF61010DA999EAC894F73831D6B1CA289E88AAC324EDF143DEE1FCE14318A14FA9B7C9F5F8D513BF88D78C45100A8F5A631863F8871FB843712774D431053A7A607B703C4D706C3BB8D43A772A1D3BDB9D3ABB3A74EA62E03FD302123765BA8A1992D9EBB8943E6B64B06E9425FDB2D1ED1AEAE451F7C73905F670FFE3C17D9B635A895B50F84C2E4382583EB79F137AE9EB69E6855E16F528AB81C731DDBEA837221839C8763BE028AA9DA8FF5267E26E4FDA793C3C4D3CB48B81719165E1D62CDE2C53DE03D108135A110CD0DAFCBC7E0FB537C2E0A5EF78ACE91CE9D41EE74E13E7BAEA7D0C8C41DA1F900D7BFE0B65ACA311A84360B1E4E51858726E2B7F1DA360D1FDBF863B5A0A668E68CA6DBC9112D601A61A44D332C89585D9648AB432CB967317138BFB8218144BCA6C67B07F58ACC7E48081D9E4B60097F46A800605D1BE58EC8A96F7B6624E8B591C447EA9F34B9DB4D411D180CF90F1247DAE4B3087C86C867C8ACCE5DA87D9A142DFC212534664AACE52B0D5E4DB21DD61C7E0DCE720FA8696D829E126FB1005DBAC1E6B03E8AB2A3B45BFD9A77883D2E87B186F5931E0FBFC0EE1930DDA86DBF9AC0C70F07EFE561A1CAEE0635DF04CEE2CD22D2D5D452E36AA33840EDDC51298B8CBEE1FEEAF4D7BED62B5BAF9787F7DA5EA3CA997CBBBC69BFB8F750579120315980FFCD25EFEF2E2FE922BFFC72EA3A97A9CBECBC0C2B4261EE39BBBE5C3E393E928FFFDFAAFA6037CF9B0FC87E9D85E2C97AA61B59874388119197228693CE370E2C46371F57077BD7ABAB9341D8D87CFD78FABEB8B5E78C5FAAD7510EAA6FAC47DB7FADB3F96D7C63DB7ECD769D2D39FF64227903821A42700F64CF0D8180D48A5EB7F7F7ABCB87C32C5055AEBF2D3EAA9981A8FBDA01C7A30A383C0CB644E68E8968F0F97D785BC31ABAE66DC961785A05E998ED715CB8B667DBE7E7C7CE837A2E0B3111D8614A07342634A19B750BCFED7F5E5937A4CCDBBDE89560452F23A517F9D487E26A427DA6122130F8CB0E66806465C6C3483F354E85EBD260571DBEFD0CBB8E2C43DFB697565DAAD9FEF9558632398FDD7E093836AFBE5975B503580C12FA85D2083B77FE830302C8189C7A598F8379F8D47E6E65E28AF1917BEAF8DC0E222CB927558F65C73EB168531C25762EB3478C933A13DD7F166468EF2A492CD615F63BD420BCD6777458F86FBA20FC3FC7BD1F2376FE4C6B491AECE0979D265A648FB0F12E147F482B0994F1814FBC2382B46348C73F966228CD7E13E8874CD132A1A5E6AE011A83F21E65CA13D8AF17D84AEFD26DF6603EDC83CD49F12AE5C747D74BE6064A55D84A819753D3EAA5116CA41E2531972EB46584DD4427040A1EC243B0A2646901C459F9A7CB9AA7304725342867E84CB39E2586A4A9AB0D01803594F996159185564D8FE3494185C653281C1576FB5F9876A68D94290B0E07C1B51E1E8C18242CC51861214888111C404EA474321A92CBC261712B532C395722D262AF50592933F886A5B07F86C9D0F7C31D7F039D9AC805918153E4F6D662C83F4B762EF729BE885462E0A094E53CA467800DAE6027426F6F4F9434CAC316617EBBC7491BD0CB275B091ED388A0DD1C681E0A9D91F41F8D4E37262024866911A9FE5A27602A8D97002E401193405C6BE7224B4705C3912DA6FF2F1A6F6318852FD00B6C16057650741B39A38204A1F51D23CD4AD13558B7E905FDE5531DAF20C6FC36653C8A60FD4EFF76A142A77EA809285112693B2674DBEDD388C4C328FE4D7910D0659F4AA77283F824BBE168CA7D70794CC8F2B79FC98987CBB898A32B5E43DECE9CBD7060252951D44FA6AE253E297C8C4B87224F6AFC9D7AB3A93C912F85EA96ABCDB1F2F651C71896787B920B53E783AE6DEB88D9111A4A9AD834F619BC23D2CDB3ED8E22BB3BDA5477899962158BD373BACD0F0DF1F4D58F87E34D29BEAD7EBA75EBDC8481BAC2E823798C3758BF722D349E151E94C1CEBE32E75DC78987CBAF6109F44E4C0377F54C2D1FE00502320F5733CE672D7FA6C909E74AFED2EFC96908E53C5C3424E7A017E8EA84B37749D4CAD9C8C30A35A7BD9F03889BE7239CD5112F09088F2B4A7ED5511E628897B58CAE230A9E541125681A4CF621C1BACB7B03FC699A67A6C8CB4CEFAE9A2E9A5B07E2AC64850E477631C4BA2F4E4CC987612AD9C8C2D5662579B7C7F72FBAC25F4B88FD1C02FC5977E1C0BD652782448A67FE4202734606C7114C6C768B16D42A24FB871323B6CD61E34DB6F974EF68079AAC3E5533D58A6417835D22044E4D5C997E656198CE4DB659F6FD15A3E22A68A334578CC86351A14C87C3AC121354FC18003E47C84F9048EC1C99C870AC1B75A4F315591B88473A92A8CA1E519A92286D729885E0BFF631DB5C263736A62A8B175680BB7E65408EDCC1D5C9ED4830C8C2C442767ED2087C133196421269E5301E2A3E9E916E4010488636064F9E13AD6E4DB7530D3C9A5878D516832CE60C042A79204853AD4E2D1312D88400346964760904C3810A2F54E2F9BAD17922DF129DDCAA3E59DA473641BFB6651D9B1463234E9D5221F7350B9BD85031032FB5B1289D4E24C060C5A68A2C9BB31BA823E3FC6C90AD491462A781352744239690241B68F2B1015B2B7ACC891244796168981D1E445EA4E23859B8D663B82CC90489E459DBCA8D1449008F200A7A157E8E5944235A1511CAAD865A20C609A2B940B8ECBD97CD6040E957CBD25419288E07B2B050D72A5A52151F9B0C814AAA3643D0FC44B11E48188B2860456EBA0FA44DDD3546E7C172012AC3F848610E3DD0051E29C1F34A49A335288127B82AA2154AEA8100DAA7F68AA13FB32A87E6588A72170F93588B7E836D94234EA4C2D196A4F0111A92D4B74C3CC5C198103CD5DE5E964165F9387F0BCA1660966ECB470A225C1BDF02C51E1728DC44D2D6A5A02E59614AA4F37F03A292D83D580624A1F9A321173A2752A85BD52E4757D819719B02BC85A2D54675600095699703233A61C0FB0AA9833DCB2AF8E3A53B3CF82B9B4C4990499E129557C8B3B5EBEBD067D21C645017AA235740AC7BD2A780AC37BBD1EB574822A5CCAE05D4056D4B61E9083802838E7C280746E3F17F8C340963AB49E8B5701B45D1DCF82E3198C68C1704C7580965683312CF8365345C4699BC1C9DF129F41C93338E9ED9AAD9AEDAEDA2D4463689574CD782B22367496F411461D082D00F4802E0001C77F4B0802A60D9C0ADBD21B2D410706EF917AB45A7B04F48857B541F489EFD723A20BBC5ED2FA7548B31968ED11D8B15BD508C9B5BB5F9F48CEDC0C397EBBD3BB67641F6EA05F348EDEFC5189D2D59B6904B7D16AE913B573B70E8BFB7545B5876EED0AC88C4CC5BD604CD6AF2B0433B241660CE421DBDE1DB023ADAA0D922B6DBF2E919C6707900FD0D113E813BD43A87C6FA17209659A519D29B47448AB13E8304B0DE7AFA8EC0CC89F11E05CF068ECD278C1879121511FAAB89C1BF43CA775620057622A31E6EFC4FA4D09FE0E4CDB971DFA01740A03FA42EF3CC6B5A1D57D8C6947733AD5D221AD0E6306C47A740BEF27D6D22F2D0E65605B6097B2AE3D033B910DD33590BB13A487E9BCA278C5A9C52F8AD5C3F893C8364DACC5138A45D1FA34D26DBFD40E38BA9E813D75D44D917C75FAF78EE49D33CC6106E845A2EB1ED0DB44DD14D1DFA47FE7881E26324577AB915665355557B5AAAAC90234A28A4A6DEDD5ED066CF1218E796BFC6EADE68DEFCD96EF0E0DE7CDEE8196B7D8E5736CC396F90CDFD505434BD3615BFC01954DC1185CA573B6D98CCB7AA3C26A5CD49AEABB0E9D1EAAB0131FBE5BD49B789D09B3B211EA6D7CA73E197E230F3C38DDDE1D8039AE927DDE20B76F67F026B8DA99D7B333B8C7ADDBBB446963AA6C0A6465DAB77B20BBD2810546B9A3D35839AADBA0DAD375EB91C17775C21BC0C0BAAA36D8E31745D0648F5D15E99D6FDBC20A1AE90D03A3D24BC3AAA62B6CD000C6652BB42ECD97EDCEFA7540F5E6716D2955E79D2F56EBAF6817D084F34551648DF6F92188EE924DA105551977C17E1FC6DBACA9495366AB7DB0C61628FFB29ACF5E77519CBD9F7FCDF3FDBBC5222B49676F76E13A4DB2E4257FB34E768B60932C7E7EFBF62F8BB3B3C58ED058AC397D4DB4EBAABF942769B045426EF1E982D30F619AE5D8F2EB4B805FEEB9DCECA462AC5D98C236A1FA9268FA250F5865B450D5C07FD3BBB9EA6920D53D7BD3771F8AE6ECB0E55DF98A3C702B2E572D2AAF0A780C52E0D1FACB243AEC62B511A0BA36FE9FAF4F52CC29E031E0299014730A559BC99BB62C253EC78227261602C7594B8C043535FC643B7D829E27C765D8D1A30FD3CBF4EA0C737A786DA46C880D16B2EC685256209A4C9639CD9B8C3E682F086C936CD78720352EC3AEBDB41AD45E264BA679BE1066B5081A0B093504FC1661C814A4C8A14B578C822C61CC200AAE390C429153048C29CB20CF511AF3C480EC31B1AB3F7E3E05AF321B75A2058A6E3629CA320140AB448B594ACE80E5867119D6F4C0668A79D654C1468B791EE975343DD21F35D2AB6D760C801E3E0C36C07955C56160BE3F14D77B27BCB1D91C22B4790A450C5395B1FE4A8E94C4AB2C0BE8269BDE16BEE112D65F5012B6A547644326C7A69B532BF7BEF282C3245B6D26AAC7E3849D84EA4DB9565AF5D385D0B6A41B3559D2F99C29374ED7591EEE0A5CBDD8258738E7698A7916E35BDE89B4883758A033FD5F93439AB57E8096B0FD828A715B7EF9876D587AED4FDE682982C46CE87C0EA26F68190A23DFA45AE06479F62F4F6A36DD42496E4E87659252A6395D7A67C052031D6AB434567901FDB27C88799654AF6360B2F0395668818F0FE5FEE3322CC6041F4303A3D1245BB496BEC3CDB593A659B710806B5544A196D62579104130C86558CC50E2F6C6CD4FC8134ED34BA09075912F50B43A48D57D928B1BE044F6E26AA340772D8DB536A7048A99D63A8AB436B2E9E6D41EC2E832D9ED83F83B4F8D4DF71B4D1D4DBFD13CEA8D668BBF94D9912264806176A408D73CD6BDA6BCEC59EF531C1DD77924F14872844842EC353AC1481953C21E43E06AC30008FE960C014DAA85927288229952936AA1D0ED823012D43992643309AE631C0F409A0475B239ADDB20CB6F936D2828744CF2348AFD5598EDA3E03BB03365332C147B94EEC22C93345736DDC3B98EA687F3A38673D6B3B513A833517EECA1BDADF23000EF12708E41DBF4B0E261E5086185F370EF842B6CCC2F7B6069AD3D0CB2DC26F136CC0F222430C9362A560E91AA533D3EE8687A7C386A7C609DA93AC10313C8CF1E1DDA2A0F030E75785EE942C58E8ECB8B02E80EC4FEFAE309EDCAB80DA5B53D7F79C76458B6F0FEB0FB82FD5BA436561956D721103526D946E1DBED8169CB245B5D1BC512A53A71FCEB9EE66505F952D74E461FF6CD6BEEDC85CA5EFDCA7B0B5F65C80785DC4B99B674C1DE13B2C6BEECBC0BE2431029E715906DD3EA382B50506C2F4DB4BD82134758FD70979A928BEB531CB093274152AC6C751F03F1BEB34EB4C0D4FD3E4DBE05115E77A53DA690674F15922F31CF9EAA7CD1CBE74C7FE1DBACD7F2A5AF9867611283A908D4AA345B2A677F82E8E054734A7F47C4FB9FA553A59953F9184491D8F5559AC5AAF29AA7C15AB413AB12FDF64347D36F3F8E7AFB411D6D3BED3C40BF61834D87A2DE30FB0DA2C248DA5C9D6AB332E230D2D2D6A54E1DF3B8F42E7885362D4CB2053734C21A404FC89AE670F9532C1A6B92146B19588232B0B4DFABA4222926D9569E56280D830892A92AC77A870050E473FCAAA5A3E957ADA35EB5AAE8879D962D387CA3C1BAA5AA38CCC245262B78FEC3E58CBD29F790E021E10821817924A6132A34EFC8D803434BDD61B0817C1BFF2D9E7636E9B6D4A86D1044B0CEB25194C84F99473EC79CE2DFD0F7CF417440028031C91607B3D106A2C5245B582EA07F42B498644B6C2D8509C2D62A635ADC622ACA832B651E0D3ED4715F3BA183229CAD0136286B0E830C0E8FA349245DF15EA64EB5D86096D18AC5ABA22AD162C5A4D119009548C8B2D9FCBA0CA353D59227069F63751C7F54A133DC1C151FD301AFEB6B5B57174B4B1254175C409B8C71AF975C5D0EAD0E5F4A1F43612F52A75A712471E377337A9A7E3773D4BB992517A3BB93CAC292E86290DC5A7D18E5C5ED624C5B2069304DB2A52DCE95047D4CB2959D8A4CA94E1C17D0DDBAFCF63FA927EF1348D70675EA3437107E91F08BC4112E12F5931D9DD607F825128395415571A035E1085C4B8E6D13E6E3177A2CF6587C5458BCAC9EC0E9A3AB7757D37F4F68EC11C023C011220036B4FE352C36AA29F4BC9B0108B410300082D6DA03592838DC14F7375C77B9DD2F7689B962F798DB6F1F3F8411FA940A879D75A2073E1D4D0F7C470D7CD593729D408F54EEECB9787AD7AA3EE8A2314FDE58CD43A6617B4F0C32E97B789D10137CDECF003015F586C14B97A7EFC7884E7E1FACA7E7B1ED77896DC479A5AB4B5959B98B4F99A2E2403B5FE7B6F9D4D30AF29616B22C544D77A136BC2AE6E1CAB0BDA70657ECBBB5DD30ABA1D0D51956557B18F472850BEE02A530CF5B487CF15916A885F55D915A9D38CD29A7374AF178FA83E3297D07BADB6120AEDBE52C10AE370C7AD6EFAB49C0C766586002799E4CC28426D963828EA6C784E3C004FE9176395CFE73F3BEBB222A3E53028A7EAF7845033F320F74534DAC7AB05EEE102380506DBD7077D4DFB565E92948B7E056D788A5169541CDD5F9021C20F3316CA2543F37D1AB5A436133E5CC435E037D0750ECD983351907230B71D74FE4AC99BA4CE24D883F3CBBC9EE0F51F47EFE124499148257D9E6DEB2414DFA9E89631FF57A6CF1A5140A1AFA4D029D0FD1EB291B1C2907F201B2788C02A269786F2121B7B6CF24DCADFA3EB82A601EAE16E8749654CFCEAE48F597048EA97E423A8C04289BAA1FFB4A3B28BE9307618C52B148AD7ED094FA775625E0010FB6E82ED9A0286BEAE1574D7741D90FD93E582372DDF2214C33BC050FBE04192245E6B3A2F1DFC20D4A8B1DFAF7ACE8657A69F61FD16514227CB55E15B80BE2F00565F953F21B8ADFCF7F7E7BF6F37C76118541864F71A397F9EC7517C5D9BB75E97C14C47192974D7F3FFF9AE7FB778B45567E317BB30BD76992252FF99B75B25B049B6451D0FA657176B6409BDD42AC4EC91A5179FB978A4A966D385B07663FD51C2D94CFE1CE67E217DF5D7D8F8382FC8730CA890252167C2E8420C9A9BA58457A28F4C21DFA6792FEF686AB94F13FAFD04B18877248F9F3BF2149E82A612CE46AA692DBF38558F11C907DDCD8F7F3108F61890B1F518CDD73D16619E40553312E85CA56CC6758BCF18D402DE28B56F2F8FFEA03F1B7205D7F0DD2FFB10B5EFF274B294F0F5A42E4165043C88825FE79631714592F3AA62F2571B98937E8F5FDFCFF94D5DECD6EFEFDB9A9F9D3EC212DE6CEBBD9DBD97FDAF60DB707250CE0AD595E5EBD5AB685DB7FF6A425EC3C99AEB16B9FB0DBEC4C87D965121A5FC2BC53070964A00E326D17B3AB346C17BB95D401177EB6DC04B770390F5BA2551AC61C4A4B313A66F060025D26F2E20C4C6B9F2307B46A4F2307B4382F2B77F45C3657F4AF7240D2C3B7876F00BE090669D19B14F3E03D84AA585F3CE03DD0E610216ACED64F6E28D51CB920466F339C324869BA2045DF4B7740A9BC8A72B632509DC75E63AF2BF651D8AB2D48A7CFD39A2EBEEF4E09B0D9539910BCCE0A192924A532DBA7B283D6456A848F1A8ABFB2F2CCE0ECCF0582602B86F7F39FAD27383993713A790492BF2687347343D30577CD79BABDECB175FB481FA5E3444E3E07D137B40C73C5F26B86F2A55D8D3BE5BAB1AB71469398983BA3C45878F713A7925A6DE5DD73D521176FEEC601DB5DB91B016A96D95DCC988B456BCC2715FB4C3ACE096A2034A5EE51035137125A234A06026B76B859BE5DE4E270537CB7A8BB9451858B5987BB5061A354F50056BFB1F51B5BF05CB23435313897C4E5FCD67688ADADD1C268B6AEF9A3308F18C3220626AF858BD208CC63053726CED4DF0F4569779071BD0BC2C8CD717C761DE3D2BD26C26D90E5B7C9368CFB4E8369557CFA9884B3315FA2741766594F85D443BA877400D2191B551DB03336A01EDE47C09ACE9AE87C7617BCDEA2789B7FC5665F4E74528EE6D9DBB75EA3F4F063438733CCB69B284C55F365D918003FA2A439EFD1202053D643203BB6F136CC0F0D1015F329EAA0FDE5FD897898F93DC18CF12467CCEA7573BC29EAA7B87089630FDDB45A2F0DC7DD7DC3C0D72EDC734A435EBE54C15706FAC675BC19F80BF84EA337FA9027A97B91707463DA0484B09B1D55BD3E870D0FA5C8251DBEDED4EC75D841DEDD7675914DC8391A97FE77C577417C08A2312676FD7E9A11FD3F76BC8FB49792AA5E2F081FF6429AC4517661A950BDF33610A317FB7D9A7C0BA2CADBBAB3F6561172600C41291DE3157AA389F5BD46AFDE6F1C685C09F9B33F0DF681EAE5C881C8574F4A0EA54FBCE669B01E8E7DBFE7FB3DEDF95438D1E160A9AE38C0B152B9F3D16E36CB527E9F29EA7F1D54695AAF97691E2ECAA84847E2277B17BC0EBC71A5F13707FECAB497B2E49D4617A63144D2966EAE76CB3D6CEA881A116012A0D5615B5D12F44BF5EF68A9365E2C899C69574B52CC2F97CC0907178CD9C104EDBFA3F453DC4F71608A5F7E0DE22DBA4DB63FC8CC230D842C25BAF9D995E4A8E59823D582FC74C662D1E19F83E880DC58183F441B87D4EED13F1D522B21AC945737B0E3022E183A1D47D478AAD611EB34CB711D45CDAFC7ADA7ECB687E501DE8C75720B2DEBF5D92591E878BD96351A3AC49D4AA288EA64BDC725549CC1E1B1C66E19F8ACFBB48FA28FEF26AF8B7767F9DEA733411EF6726EE83BB5D5E14BE9523B6403FCCD85DF2B0D72732105A3B55BF185EA03DC6250ACA110A1F30F600A7BA550ABC08C169672D93C106DA9515615FBDBEF5DB16B408F78013A3AD32F786E5CF6D9EB8A4E761D5B14AFBF77B86BA1F57A8D387CD3E36F0AFCD2D87B4122F2A90F595016F38B90E57DB40F7E8A7CF0538FB71E6FC50D80A9EEEF1177080B202328F2070E7E96779DE5D882FED7B098E0A95EB562CAFAD93EC42ED7993B03BCEFB754ACC234EFBD972B861C7D4ABD059947B0811D7C0D9D7B3D6E696EACC7F20BF3C150B9C34D2774BC059F87D8412096C6BED6206C59CA03ACEE307CB4F850C70750C77712E901CF031EE4C28729E8118F14F390C7608E6BAF04EAA0D6EA37DF398C8DD7AF3CDC1C05DC60D9BCC8B270AB0F44C794F5C0D33EBD478A5235540C2043B3D6FAA99B0EAD672BF7D335B1EE6FCF00AD7624464613EBEB7E71F08B0374BE895FDDD31F6FE2527E416006B87A38B24B4489BA6A3F6338F28863073CA92A7A3CF178E21A4F30F9EAFD9F0E53BDDA892DF44599F5D4161C80FD9EC1BC21B50630601D4807508D59A1DE27EBB0FC52C356F9407CF5CA67260CDD75BC993D2611629E9C27ECAF50F4F2A64EBB3B4479B88FC275F1D1F7F3B76FDE9C49FDC1512ABF2592A2893CAD3F48840A394078EB1E06D1651267791A145D290B4D18AFC37D1009DC0BE50C171DDCA3354531A7407514E3D5446C9EC9B758437EF99B356941CA755D70BE6046BA5D00E81BD330D35D074D2300D5F3D70CA52A69A8C1371F919EA34F1B62F225F6A9E089C75E9E1B83CCFC09067EAC596F37EED503D5930C7BF99A16BC5273834E5EF2E3079DA4590D3A79E18B214312861A70B955C30C78D90AC3E186B59509863B7B5E258774AD9EEB46837526B6FBFC21268AE5EC629D9716109741B60E36B2565428269BB66F4362C7A50F2236C663E940666C4454A9138F2F374F41BA454EE0E287939E9141C7E67353634FA3618CB4D84CA0618C35F6761AC6A4C3CE3CEBE670E807020EF6B93A860936F9D4458769CB89890F117AF5A984F5E01D197E588ECC7810D230760C6250C7FE578D1BF7E41A33785CBA95244C050B2CC7C7070DDC9B7B934805F39816B9E8EEB797D14801FBCA1743894D3EE91D09D31093AF352609538F3D04655D707C20A5622AB9B140F87125A7710A995A721EF6E4912A8F1BE38D7ED5E7938D3F7915A37A9560B4ED287DB283A143534E7D47419A710A9B0932F2341CBC6A9CAA68F1CC405549473CDE50A3A61EEEE6B595A9711EE0D97EA47E30F5C07C1CC75D1F6A2BDB4984A68A77C8063F540A4E1DCA9A19B73ACD0A2C682D4CC1849A231100DB378C10B00D34F99C14BB725269A0C1AFBD384C250E4DD4F2698E9FD8D8A334489C5263A431E4588591268D7386CD0555658FABB88C61B44EA06B861120AE3146BA671DB2727A09AA2CBD4630B399541AC633B9B11687C98DED3881A03FD4571B559C3479007F07A802F6CE91C8D1B2898234E126E7348E4081FDCD0F72F4793AC79ECF781FA6BE43351EA8233DF3B4188E7177B58FCD2BC2938C3E099C72FC1618340C0CF37D9A72EAA7A4A419469AEA319C9252EFEFE31718CEAF5D3CC2ABD24F5D78D8C69C9A08B9B899D79EB34F2200635DB0D98EFEC437F3ECE003A8670FF6C739FAE6803EF2F0D77112261F7D26F6C3517A1978E8902E5FF8501FD34B909B8BBB63C48FD1EEDFAC6560D20B38126482462770A377B66C5349DC0B76874A524E5D6124CD3052159B0012138E771D0EC38FF8E023CE452D1961CC49FC81A24E5ED4406975009F6CD08730CD7058A5E04B90C9238F6BAD502E7848CF67D7753C03C9F57CB5FE8A76C1FBF9E60B8E714F622234B99250485F20172CD007488E823ECECCB4F4AB1357897C950151AF9C1E0C98275301629EE428982F7DFAB4F48956261127C910659CA327CB9AD04BC4D94CE8134DBEFE439CF1BFF4252E17FA145340FF2DF69453FA149B097DA9C9D77F886A3BD237683A44BECCD253AE2CDE24D25506449BE4E9895F7E0DE22DBA4DB6007D260FFA449DADFF4A6D2B217DA3CE81BE40330D4497BBF8928597CB06C597296130C1A90D813CBF690638BD715E68DE18753B5A9BA0A7CF3D01237D83CB85BED31430694C7563A1987B6DF34E4F9C9E5048B4693A44BACC3298CD3420B03C9D6906389FCB3C43A8A8F633306054B94AD820050CFA9F685472F79374B0F77196D92AA45EE9B85CD59A5417103EC5282F924AC084449A31E578ED40153749D462591DA3F8129326E9625C3D4EFBA92AC29ACD826F8C4143C5D03F40335BA303756796ABC9AB4865BD2AC96113496FB7B510325B71308A43368F8BE402344E1DE905DE66F14D83A60C708CC75422094E9B55472CD1340F8E6C62CFAE5403EA9C364CE9DB5C1A6843DB5C282087B3511DBED1426488D6993990F80E393381080840137571129C3453DE6E9555D96497CDAD37AFADCD05FDFAFBB13DC5D8361BC8D6D6AA1CBA550EEC0CD75CFAC4A32CBB5E03CDD6F867F74464799B5DD663935D36B39299D666C286675D8471CAA6D66EB1ED8D85BD674F6054410F50A0AD7A4F5127A8CC1DE394B5688AA386566736AA06C2BE83804B24C361953449B324CFBE7639056FDAEC191D5D4C415F34A0A97A9F358E75E160ACE4BB4E6B6932E794A4AFDBA3C1BCBB554B8B5BFCB24EA4C9906711A440E81C907820E2CE0A090ED1A436B50138D0248A0397E1B6C9B52B8CAED1B0CF8C936DF9240DAFCF5535ED56383F40CE2132E747D16CDE454109D5C3EA4E004ABB6B1ABDAD51B60CB0B9EFC6E2E84B106F360EB4B0C5AEDC896EC41DB797B5688A1BDD48307356A9486DD6D0EE5440FE78BE5144AA74A74D56EFDA74C6BB3D15FCF19B4A25A6BDA5A039A7BD204EDC54D6E252D360A571E6C98DAF720FA0B124ECBD0918A7A9BC451CB4CAA84DE69C8013779F47D61892E2A8698DF197B2710AFBB023695EF5C64A6DE354E79D2FC83D244D287EE6491A6CD15DB2415156A69E2F1E0F317EC387FCBA42586C6A12E705CD18AD399BAABACC4DFC9254F65D02475511E1E9993B94079B200F2ED23C7C09B08E97AC5121A8F1763EFB1C4487A2C8F5EE0B2A76910F877C7FC88B26A3DD9788DB386013B1B6EF9F2F249ECF1FF6A5E98C8B26146C86F8D9A387F8AF8762BF5BF3FD0178FB464102DB9ED167C0F058E6F839B0EDF79AD27D226E18548468F7D526734F68B78F0A62D943BC0ABEA12EBC15F87A8BB6C1FA7B91FE2DDC60B05511D10F04DFEDE75761B04D835D466934F58B9F850C6F76AFFFFAFF01C9E8507AB3810200 , N'6.4.4')
