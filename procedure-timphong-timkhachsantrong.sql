--procedure lấy ra những khách sạn mà còn phòng trống
alter PROCEDURE sp_LayKhachSanCoPhongTrong
    @IdTinhThanh INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT  ks.Id,
        ks.IdTinhThanh,
        ks.TenKhachSan,
        ks.DiaChi,
        ks.GioiThieu,
        ks.TieuDe,
        ks.GhiChu,
        ks.AnhDaiDien,
        ks.IsDelete,
		ks.SoSao,
        AVG(p.GiaPhong) AS GiaTrungBinh
FROM KhachSan ks
 JOIN Phong p on ks.Id = p.IdKhachSan
    WHERE ks.IdTinhThanh = @IdTinhThanh
    AND p.Id NOT IN (
        SELECT dp.IdPhong
        FROM DatPhong dp
        WHERE (
            (@StartDate BETWEEN dp.BatDau AND dp.KetThuc)
            OR (@EndDate BETWEEN dp.BatDau AND dp.KetThuc)
            OR (dp.BatDau BETWEEN @StartDate AND @EndDate)
            OR (dp.KetThuc BETWEEN @StartDate AND @EndDate)
        ) AND dp.IdTrangThai IN (1,2,3)
    )
    GROUP BY
        ks.Id,
        ks.IdTinhThanh,
        ks.TenKhachSan,
        ks.DiaChi,
        ks.GioiThieu,
        ks.TieuDe,
        ks.GhiChu,
        ks.AnhDaiDien,
        ks.IsDelete,
		ks.SoSao
END;

EXEC sp_LayKhachSanCoPhongTrong 1, '2024-01-03', '2024-01-04';

ALTER PROCEDURE sp_LayDanhSachPhongTrong
    @IdKhachSan INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        p.Id,
        p.TenPhong,
		p.IdLoaiPhong,
		p.Id_ViTri,
        p.TrangThai,
        p.GiaPhong,
        p.IdKhachSan,
        p.AnhDaiDien,
        p.IsDelete,
		p.MoTa,
		p.SoNguoi,
		p.KichThuocPhong,
		p.SoGiuong
    FROM Phong p
    WHERE
    p.IdKhachSan = @IdKhachSan
    AND p.Id NOT IN (
        SELECT dp.IdPhong
        FROM DatPhong dp
        WHERE (
            (@StartDate BETWEEN dp.BatDau AND dp.KetThuc)
            OR (@EndDate BETWEEN dp.BatDau AND dp.KetThuc)
            OR (dp.BatDau BETWEEN @StartDate AND @EndDate)
            OR (dp.KetThuc BETWEEN @StartDate AND @EndDate)
        ) AND dp.IdTrangThai IN (1,2,3)
    )
END;

EXEC sp_LayDanhSachPhongTrong 1, '2024-01-29', '2024-01-30';