# m-duel-project / frontend-realtime
이 repo는 2016년 10월 12~14일간 예정된 m-duel 프로젝트에 대한 frontend-realtime 작업을 수행하는 repo

###web app
클라이언트 디바이스에서 실시간으로 예측을 요청하는 데이터를 받아 실시간 예측 분석을 frontend에서 실응답하는 작업을 수행  

- 디바이스로부터 받을 데이터는 postman으로 json을 Restful POST 방식으로 전달 받음
- 전달받은 JSON 데이터를 azure ml로 요청해 실시간 prediction 응답을 받음
- 받은 응답을 table storage에 table로 insert 기록
 	1. user id, guid, reponse JSON을 그대로 저장
 	2. storage explorer 툴 사용해 데이터 조회 및 디버깅
- 예측된 결과를 DB의 member 테이블에 update 수행
- (option) 1초 간격으로 예측을 요청하는 디바이스 시뮬레이션 수행 - console appliation 활용
- (option) apache jmetor를 이용해 초당 100 reqest/sec 테스트 수행

### ml rre 실패
어싱크 리퀴스트가 실패함.
http request로 보내보았지만 역시 400 에러.

포스트 바디를 아스키 인코딩하여 문제가됨.
requestb.in 으로 전송되는 데이터를 확인한 결과, 컬럼들이 깨져서 나왔고, UTF8인코딩으로 변환하여 해겨라였음.

### storage insert
더미 데이터를 azure storage에 삽입
MS Storage Exploerer에서 데이터 삽입 확인.

### sql insert
backend 팀의 코드를 복붙 +_+ 감사합니다 :D
_