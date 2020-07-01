
@echo Request BQ - Google Big Query environment CLI

@REM 
@REM #standardSQL
@REM SELECT
@REM   COUNT(*) AS num_downloads,
@REM   SUBSTR(_TABLE_SUFFIX, 1, 6) AS `month`
@REM FROM `the-psf.pypi.downloads*`
@REM WHERE
@REM   file.project = 'botbuilder'
@REM   -- Only query the last 6 months of history
@REM   AND _TABLE_SUFFIX BETWEEN 
@REM     '20200101' 
@REM   AND 
@REM     '20200630'

GROUP BY `month`
ORDER BY `month` ASC

set BQRY=SELECT COUNT(*) AS num_downloads, SUBSTR(_TABLE_SUFFIX, 1, 6) AS `month` FROM `the-psf.pypi.downloads*` WHERE   file.project = `botbuilder` AND _TABLE_SUFFIX BETWEEN  `20200101`  AND  `20200630` GROUP BY `month` ORDER BY `month` ASC 

@rem bq query '%BQRY%'
echo %BQRY% | bq query --use_legacy_sql=false --dry_run
