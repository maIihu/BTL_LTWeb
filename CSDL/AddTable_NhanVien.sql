create table NHANVIEN(
		CusID int Identity(1,1) primary key,
		HoTen nvarchar(50),
		TaiKhoan varchar(50),
		MatKhau varchar(50),
		Email varchar(50),
		GioiTinh nvarchar(5),
		Img varchar(255),
		ImgPath varchar(255),
		EmpFileName varchar(255)

);
